// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Windows.Markup;
using System.Threading; 
using System.Windows.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Xml;
using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.DataServices
{
	/// <summary>
	/// <description>
	/// Tests the XmlNamespaceMappingCollection
	/// </description>
	/// </summary>
    [Test(1, "DataSources", "xmlNamespaceMappingCollectionTest")]
    public class xmlNamespaceMappingCollectionTest : AvalonTest
    {
        XmlNamespaceMappingCollection _ns;
        IAddChild _addChild;

        public xmlNamespaceMappingCollectionTest()
        {
            RunSteps += new TestStep(CreateXmlDataNamespaceManager);
            RunSteps += new TestStep(AddChildTest);
            RunSteps += new TestStep(AddChildText1);
            RunSteps += new TestStep(AddChildText2);
            RunSteps += new TestStep(AddChildText3);
        }

        TestResult CreateXmlDataNamespaceManager()
        {
            _ns = new XmlNamespaceMappingCollection();
            _addChild = (IAddChild)_ns;
            
            return TestResult.Pass;
        }
        
        
        TestResult AddChildTest()
        {
            Status("Testing AddChild");
            object foo = new object();
            
            SetExpectedErrorTypeInStep(typeof(ArgumentException));
            
            _addChild.AddChild(foo);

            LogComment("Expected exception was not thrown");
            return TestResult.Fail;
        }

        TestResult AddChildText1()
        {
            Status("Testing AddChild with null");
            SetExpectedErrorTypeInStep(typeof(ArgumentNullException));

            _addChild.AddText(null);

            LogComment("Expected exception was not thrown");
            return TestResult.Fail;
        }

        TestResult AddChildText2()
        {
            Status("Testing AddChild with a string ");
            SetExpectedErrorTypeInStep(typeof(ArgumentException));

            _addChild.AddText("  Hello World  ");

            LogComment("Expected exception was not thrown");
            return TestResult.Fail;
        }

        TestResult AddChildText3()
        {
            Status("Testing AddChild (just white space)");
            
            _addChild.AddText(" ");
            //Does nothing, but should not throw

            return TestResult.Pass;
        }

    }
}

