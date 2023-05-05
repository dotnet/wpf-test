// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Threading; using System.Windows.Threading;
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
	/// Tests the XmlDataProvider properties, methods, and events
	/// </description>
	/// </summary>
    [Test(3, "DataSources", SupportFiles = @"FeatureTests\DataServices\Magazine_NS2.xml", SecurityLevel=TestCaseSecurityLevel.FullTrust)]
    public class xmlDataSourceTest : XamlTest
    {
        private int _dataChangedCount = 0;

        private XmlDataProvider _dso;

        private TextBlock _testText;

        private object _initialObject;

        XmlNamespaceManager _ns;
        XmlNamespaceManager _ns2;

        XmlDocument _doc;

        public xmlDataSourceTest() : base(@"Blank.xaml")
        {
            RunSteps += new TestStep(CreateDataSource);
            RunSteps += new TestStep(InitialVerify);
            RunSteps += new TestStep(InitialBind);
            RunSteps += new TestStep(BindVerify);
            RunSteps += new TestStep(PropChangeRefresh);
            RunSteps += new TestStep(RefreshVerify);
            RunSteps += new TestStep(SetDocument);
            RunSteps += new TestStep(DocVerify);
            RunSteps += new TestStep(ChangeNS);
            RunSteps += new TestStep(NSVerify);
            RunSteps += new TestStep(ChangeNSandXpath);
            RunSteps += new TestStep(NSandXpathVerify);
        }

        private TestResult CreateDataSource()
        {
            LogComment("Testing Xml Data Source");
            _dso = new XmlDataProvider();
            _dso.DataChanged += new EventHandler(myDataChangedEventHandler);

            NameTable nt = new NameTable();
            _ns = new XmlNamespaceManager(nt);
            _ns.AddNamespace("a", "Test1");

            NameTable nt2 = new NameTable();
            _ns2 = new XmlNamespaceManager(nt2);
            _ns2.AddNamespace("a", "Test2");

            ((ISupportInitialize)_dso).BeginInit();
            _dso.XmlNamespaceManager = _ns;
            _dso.Source = new Uri("/DataServicesTest;component/Magazine_NS.xml", UriKind.RelativeOrAbsolute);
            _dso.XPath = "root/a:Magazine[1]";
            ((ISupportInitialize)_dso).EndInit();

            _doc = new XmlDocument();
            _doc.Load("Magazine_NS2.xml");

            //doc2 = new XmlDocument();
            //doc2.Load("Magazine_NS2.xml");

            _testText = (TextBlock)Util.FindElement(((DockPanel)RootElement), "testText");
            if (_testText == null)
            {
                LogComment("Unable to find TextBlock element");
                return TestResult.Fail;
            }

            if (_doc == null)
            {
                LogComment("Unable to create XmlDocument");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }


        private TestResult InitialVerify()
        {
            TestResult result = CheckValues(0, "root/a:Magazine[1]", "/DataServicesTest;component/Magazine_NS.xml", null, "", _ns, _dso.Document, "Initial Verify");
            return result;
        }

        private TestResult InitialBind()
        {
            Status("Binding to text");

            Binding b = new Binding();
            _testText.SetValue(Binding.XmlNamespaceManagerProperty, _ns);
            b.Source = _dso;
            b.XPath = "Title";
            _testText.SetBinding(TextBlock.TextProperty, b);
            LogComment("Binding to text completed");
            return TestResult.Pass;
        }

        private TestResult BindVerify()
        {
            TestResult result = WaitForSignal();

            if (result != TestResult.Pass)
            {
                LogComment("TIMEOUT");
                return TestResult.Fail;
            }

            _initialObject = _dso.Data;
            WaitForPriority(DispatcherPriority.SystemIdle);
            result = CheckValues(1, "root/a:Magazine[1]", "/DataServicesTest;component/Magazine_NS.xml", null, "Hockey Digest", _ns, _dso.Document, "Bind Verify");

            return result;
        }

        private TestResult PropChangeRefresh()
        {
            Status("Change the XPath");
            _dso.XPath = "root/a:Magazine[2]";
            return TestResult.Pass;
        }


        private TestResult RefreshVerify()
        {
            TestResult result = WaitForSignal();

            if (result != TestResult.Pass)
            {
                LogComment("TIMEOUT");
                return TestResult.Fail;
            }
            WaitForPriority(DispatcherPriority.Background);

            result = CheckValues(2, "root/a:Magazine[2]", "/DataServicesTest;component/Magazine_NS.xml", null, "Car n Track", _ns, _dso.Document, "Refresh Verify");

            return result;
        }


        private TestResult SetDocument()
        {
            Status("Set the document");
            using (_dso.DeferRefresh())
            {
                _dso.XPath = "root/a:Magazine[3]";
                _dso.Document = _doc;
            }
            Status("Document set");
            return TestResult.Pass;
        }


        private TestResult DocVerify()
        {
            TestResult result = WaitForSignal();

            if (result != TestResult.Pass)
            {
                LogComment("TIMEOUT");
                return TestResult.Fail;
            }
            WaitForPriority(DispatcherPriority.Background);

            result = CheckValues(3, "root/a:Magazine[3]", null, null, "Organic Gardening", _ns, _doc, "Document Verify");

            if (Util.CompareObjects(_initialObject, _dso.Data))
            {
                LogComment("Initial object and final data value were the same");
                result = TestResult.Fail;
            }
            return result;
        }

        private TestResult ChangeNS()
        {
            Status("Setting the Namespace Manager");
            _dso.XmlNamespaceManager = _ns2;

            Status("Namespace set");
            return TestResult.Pass;
        }


        private TestResult NSVerify()
        {
            TestResult result = WaitForSignal();

            if (result != TestResult.Pass)
            {
                LogComment("TIMEOUT");
                return TestResult.Fail;
            }
            WaitForPriority(DispatcherPriority.Background);

            result = CheckValues(4, "root/a:Magazine[3]", null, null, "Cosumer Reports", _ns2, _doc, "Namespace Verify");

            if (Util.CompareObjects(_initialObject, _dso.Data))
            {
                LogComment("Initial object and final data value were the same");
                result = TestResult.Fail;
            }
            return result;
        }

        private TestResult ChangeNSandXpath()
        {
            Status("Setting Namespace and XPath");
            using (_dso.DeferRefresh())
            {
                _dso.XPath = "root/a:Magazine[2]";
                _dso.XmlNamespaceManager = _ns;
            }
            Status("Set Namespace and XPath");
            return TestResult.Pass;
        }


        private TestResult NSandXpathVerify()
        {
            TestResult result = WaitForSignal();

            if (result != TestResult.Pass)
            {
                LogComment("TIMEOUT");
                return TestResult.Fail;
            }
            WaitForPriority(DispatcherPriority.Background);

            result = CheckValues(5, "root/a:Magazine[2]", null, null, "Car n Track", _ns, _doc, "Namespace and XPath Verify");

            if (Util.CompareObjects(_initialObject, _dso.Data))
            {
                LogComment("Initial object and final data value were the same");
                result = TestResult.Fail;
            }
            return result;
        }

        public void myDataChangedEventHandler(object sender, EventArgs args)
        {
            LogComment("DataChanged Fired");
            ++_dataChangedCount;
            Signal(TestResult.Pass);
        }


        private TestResult CheckValues(int expDataChanged, string expXPath, string expSource, string expDataString, string expText, XmlNamespaceManager expNs, XmlDocument expDocument,string stepName)
        {
            bool pass = true;

            if (_dataChangedCount != expDataChanged)
            {
                LogComment("FAILED: DataChanged count value, expected:" + expDataChanged + " actual:" + _dataChangedCount);
                pass = false;
            }

            if (_dso.XPath != expXPath)
            {
                LogComment("FAILED: dso.XPath value, expected:" + expXPath + " actual:" + _dso.XPath);
                pass = false;
            }

            if (expSource == null)
            {
                if (_dso.Source != null)
                {
                    LogComment("FAILED: dso.Source value, expected:null actual:" + _dso.Source);
                    pass = false;
                }
            }
            else
            {
                if (_dso.Source == null)
                {
                    LogComment("FAILED: dso.Source value, expected:" + expSource + " actual:null");
                    pass = false;
                }
                else if (_dso.Source.OriginalString != expSource)
                {
                    LogComment("FAILED: dso.Source value, expected:" + expSource + " actual:" + _dso.Source);
                    pass = false;
                }
            }

            if (expDataString != null)
            {
                if (_dso.Data.ToString() != expDataString)
                {
                    LogComment("FAILED: dso.Data value, expected:" + expDataString + " actual:" + _dso.Data.ToString());
                    pass = false;
                }
            }

            if (_testText.Text != expText)
            {
                LogComment("FAILED: testText.Text value, expected:" + expText + " actual:" + _testText.Text);
                pass = false;
            }

            if (!(Util.CompareObjects(expNs, _dso.XmlNamespaceManager)))
            {
                LogComment("FAILED: dso.XmlNamespaceManager value, expected:" + expNs.ToString() + " actual:" + _dso.XmlNamespaceManager.ToString());
                pass = false;
            }

            if (!(Util.CompareObjects(expDocument, _dso.Document)))
            {
                LogComment("FAILED: dso.Document value, expected:" + Convert.ToString(expDocument) + " actual:" + Convert.ToString(_dso.Document));
                pass = false;
            }

            if (pass)
            {
                LogComment("Properties are as expected for " + stepName);
                return TestResult.Pass;
            }
            else
            {
                LogComment("Properties were not as expected for " + stepName);
                return TestResult.Fail;
            }
        }
    }
}

