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
using Microsoft.Test.DataServices;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.Samples
{
    /// <summary>
    /// <description>
    /// TODO
	/// </description>
	/// </summary>
    [Test(0, "Xml", "XmlDocumentTest")]
    public class XmlDocumentTest : XamlTest
    {
        ListBox _testList;
        XmlDataProvider _dso;
        XmlDocument _doc;

        public XmlDocumentTest() : base(@"XmlDocTest.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(InitialVerify);
            RunSteps += new TestStep(Test);
            RunSteps += new TestStep(FinalVerify);
        }

        private TestResult Setup()
        {
            Status("Referencing testList element and the XmlDataSource");
            WaitForPriority(DispatcherPriority.SystemIdle);
            _testList = (ListBox)Util.FindElement(RootElement, "testList");
            Util.WaitForXmlDataProviderReady("DSO", RootElement, 30);
            _dso = RootElement.Resources["DSO"] as XmlDataProvider;
            if (_testList == null)
            {
                LogComment("Unable to reference testList element.");
                return TestResult.Fail;
            }
            if (_dso == null)
            {
                LogComment("Unable to reference the XmlDataSource.");
                return TestResult.Fail;
            }

            // Needed for slower machines (like VMs with low memory)
            int retryCount = 0;
            while (retryCount < 5 && (_dso.Document == null))
            {
                retryCount++;
                LogComment("Pausing then retrying (slow machine)... #" + retryCount + "/5");
                System.Threading.Thread.Sleep(1000);
                WaitForPriority(DispatcherPriority.SystemIdle);
            }
            _doc = _dso.Document;
            if (_doc == null)
            {
                LogComment("XmlDocument was null");
                return TestResult.Fail;
            }

            LogComment("XmlDataSource and the testList element referenced correctly");

            return TestResult.Pass;
        }

        private TestResult InitialVerify()
        {
            WaitForPriority(DispatcherPriority.Background);
            if (_testList.Items.Count != 6)

            {
                LogComment("Listbox did not have the correct number of elements, expected: 6, actual:" + _testList.Items.Count.ToString());
                return TestResult.Fail;
            }
            else
            {
                LogComment("Listbox had the correct number of elements.");
                return TestResult.Pass;
            }
        }

        private TestResult Test()
        {
            Status("Adding another book to the xml document");
            XmlElement book = MakeXmlBook("000-000-000-X", "NEW BOOK", 22.95);
            _dso.Document.DocumentElement.AppendChild(book);

            WaitForPriority(DispatcherPriority.Background);
            LogComment("Added another book to the xml document");
            return TestResult.Pass;
        }

        private TestResult FinalVerify()
        {
            WaitForPriority(DispatcherPriority.Background);
            if (_testList.Items.Count != 7)

            {
                LogComment("Listbox did not have the correct number of elements, expected: 7, actual:" + _testList.Items.Count.ToString());
                return TestResult.Fail;
            }
            else
            {
                LogComment("Listbox had the correct number of elements.");
                return TestResult.Pass;
            }
        }

        private XmlElement MakeXmlBook(string isbn, string title, double price)
        {

            XmlElement book = _dso.Document.CreateElement("Book", "");
            book.SetAttribute("ISBN", isbn);
            XmlElement b_title = _dso.Document.CreateElement("Title", "");
            b_title.InnerText = title;

            XmlElement b_price = _dso.Document.CreateElement("Price", "");
            b_price.InnerText = price.ToString();

            book.AppendChild(b_title);
            book.AppendChild(b_price);
            return book;
        }

    }
}
