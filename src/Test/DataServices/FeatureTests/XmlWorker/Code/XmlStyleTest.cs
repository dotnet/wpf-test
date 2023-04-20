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
	/// Testing style expansion for XmlElements
    /// Also add and remove XmlElement nodes for implicit refresh
	/// </description>
	/// </summary>
    [Test(1, "Xml", "XmlStyleTest")]
    public class XmlStyleTest : XamlTest
    {

        #region Private variables

        ListBox _testList;
        XmlDataProvider _dso;

        #endregion

        #region Contructors

        public XmlStyleTest() : base(@"XmlList.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(Test);
            RunSteps += new TestStep(AddOne);
            RunSteps += new TestStep(AddOneVerify);
            RunSteps += new TestStep(RemoveOne);
            RunSteps += new TestStep(RemoveOneVerify);
        }

        #endregion

        #region Initializing Steps

        private TestResult Setup ()
        {
            Status("Referencing testList element and the XmlDataSource");
            WaitForPriority(DispatcherPriority.Render);
            _testList = (ListBox)Util.FindElement(RootElement, "testList");
            Util.WaitForXmlDataProviderReady("DSO", RootElement, 30);
            _dso = RootElement.Resources["DSO"] as XmlDataProvider;
            if (_testList == null)
            {
                LogComment ("Unable to reference testList element.");
                return TestResult.Fail;
            }
            if (_dso == null)
            {
                LogComment("Unable to reference the XmlDataSource.");
                return TestResult.Fail;
            }

            LogComment("XmlDataSource and the testList element referenced correctly");

            return TestResult.Pass;
        }

        #endregion

        #region Run Steps

        private TestResult Test()
        {
            Status("Building list of elements to verify");
            WaitForPriority(DispatcherPriority.Background);
            FrameworkElement[] list = Util.FindElements(_testList, "listItem");

            if (list == null)
            {
                LogComment("List is null");
                return TestResult.Fail;
            }

            string[][] expected = new string[6][];

            expected[0] = new string[] { "Microsoft C# Language Specification", "29.99", "0-7356-1448-2" };
            expected[1] = new string[] { "Finding Nemo", "19.99", "G" };
            expected[2] = new string[] { "Inside C#", "49.99", "0-7356-1288-9" };
            expected[3] = new string[] { "Pirates of the Caribbean", "19.99", "PG-13" };
            expected[4] = new string[] { "The Usual Suspects", "14.99", "R" };
            expected[5] = new string[] { "Xml in Action", "39.99", "0-7356-0562-9" };
            if (expected.Length != list.Length)
            {
                LogComment("Expected " + expected.Length + " elements, but " + list.Length + " were found");
                return TestResult.Fail;
            }

            StackPanel fp;

            for (int i = 0; i < expected.Length; i++)
            {
                fp = list[i] as StackPanel;
                if (!CheckLine(("Panel " + i + " "), fp, expected[i]))
                {
                    return TestResult.Fail;
                }
            }

            LogComment("List of elements was as expected");
            return TestResult.Pass;
        }


        private TestResult AddOne()
        {
            XmlElement book = _dso.Document.CreateElement("Book");
            XmlAttribute isbn = _dso.Document.CreateAttribute("ISBN");
            isbn.InnerText = "000-000-000-0";
            book.Attributes.Append(isbn);
            XmlElement title = _dso.Document.CreateElement("Title");
            title.InnerText = "NEW BOOK";
            XmlElement price = _dso.Document.CreateElement("Price");
            price.InnerText = "99.95";

            book.AppendChild(title);
            book.AppendChild(price);

            _dso.Document.DocumentElement.AppendChild(book);

            return TestResult.Pass;
        }

        private TestResult AddOneVerify()
        {
            Status("Building list of elements to verify");
            WaitForPriority(DispatcherPriority.Background);
            FrameworkElement[] list = Util.FindElements(_testList, "listItem");

            if (list == null)
            {
                LogComment("List is null");
                return TestResult.Fail;
            }

            string[][] expected = new string[7][];

            expected[0] = new string[] { "Microsoft C# Language Specification", "29.99", "0-7356-1448-2" };
            expected[1] = new string[] { "Finding Nemo", "19.99", "G" };
            expected[2] = new string[] { "Inside C#", "49.99", "0-7356-1288-9" };
            expected[3] = new string[] { "Pirates of the Caribbean", "19.99", "PG-13" };
            expected[4] = new string[] { "The Usual Suspects", "14.99", "R" };
            expected[5] = new string[] { "Xml in Action", "39.99", "0-7356-0562-9" };
            expected[6] = new string[] { "NEW BOOK", "99.95", "000-000-000-0" };
            if (expected.Length != list.Length)
            {
                LogComment("Expected " + expected.Length + " elements, but " + list.Length + " were found");
                return TestResult.Fail;
            }

            StackPanel fp;

            for (int i = 0; i < expected.Length; i++)
            {
                fp = list[i] as StackPanel;
                if (!CheckLine(("Panel " + i + " "), fp, expected[i]))
                {
                    return TestResult.Fail;
                }
            }

            LogComment("List of elements was as expected");
            return TestResult.Pass;
        }

        private TestResult RemoveOne()
        {
            XmlNode deleteMe = _dso.Document.DocumentElement.FirstChild;
            _dso.Document.DocumentElement.RemoveChild(deleteMe);

            return TestResult.Pass;
        }


        private TestResult RemoveOneVerify()
        {
            Status("Building list of elements to verify");
            WaitForPriority(DispatcherPriority.Background);
            FrameworkElement[] list = Util.FindElements(_testList, "listItem");

            if (list == null)
            {
                LogComment("List is null");
                return TestResult.Fail;
            }

            string[][] expected = new string[6][];

            expected[0] = new string[] { "Finding Nemo", "19.99", "G" };
            expected[1] = new string[] { "Inside C#", "49.99", "0-7356-1288-9" };
            expected[2] = new string[] { "Pirates of the Caribbean", "19.99", "PG-13" };
            expected[3] = new string[] { "The Usual Suspects", "14.99", "R" };
            expected[4] = new string[] { "Xml in Action", "39.99", "0-7356-0562-9" };
            expected[5] = new string[] { "NEW BOOK", "99.95", "000-000-000-0" };
            if (expected.Length != list.Length)
            {
                LogComment("Expected " + expected.Length + " elements, but " + list.Length + " were found");
                return TestResult.Fail;
            }

            StackPanel fp;

            for (int i = 0; i < expected.Length; i++)
            {
                fp = list[i] as StackPanel;
                if (!CheckLine(("Panel " + i + " "), fp, expected[i]))
                {
                    return TestResult.Fail;
                }
            }

            LogComment("List of elements was as expected");
            return TestResult.Pass;
        }


        #endregion

        #region Helper Functions

        private bool CheckLine(string precomment, StackPanel panel, params string[] expected)
        {
            if (expected.Length != panel.Children.Count)
            {
                LogComment(precomment + "Expected number of items: " + expected.Length + " actual: " + panel.Children.Count);
                return false;
            }
            int i = 0;
            for (i = 0; i < panel.Children.Count; i++)
            {
                if(panel.Children[i].GetType() != typeof(TextBlock))
                {
                    LogComment(precomment + "Child " + i + " was not of type Text.");
                    return false;
                }
            }
            TextBlock temp;
            for (i = 0; i < expected.Length; i++)
            {
                temp = panel.Children[i] as TextBlock;
                if (temp.Text != expected[i])
                {
                    LogComment(precomment + "Text " + i + " Expected: '" + expected[i] + "' Actual: '" + temp.Text + "'");
                    return false;
                }
            }

            return true;
        }

        #endregion

    }
}

