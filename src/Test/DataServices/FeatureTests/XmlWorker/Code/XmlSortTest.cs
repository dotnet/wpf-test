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
    [Test(0, "Xml", "XmlSortTest", Keywords = "MicroSuite")]
    public class XmlSortTest : XamlTest
    {

        #region Private variables

        ListBox _testSort;

        XmlDataProvider _dso;

        #endregion

        #region Contructors

        public XmlSortTest() : base(@"XmlSort.xaml")
        {
            InitializeSteps += new TestStep (Setup);
            RunSteps += new TestStep(InitialVerify);
            RunSteps += new TestStep(Sort);
            RunSteps += new TestStep(SortVerify);
        }

        #endregion

        #region Initializing Steps

        private TestResult Setup ()
        {
            Status("Referencing testSort element and the XmlDataSource");
            WaitForPriority(DispatcherPriority.Render);
            _testSort = (ListBox)Util.FindElement(RootElement, "testSort");
            Util.WaitForXmlDataProviderReady("DSO", RootElement, 30);
            _dso = RootElement.Resources["DSO"] as XmlDataProvider;
            if (_testSort == null)
            {
                LogComment("Unable to reference testSort element.");
                return TestResult.Fail;
            }

            if (_dso == null)
            {
                LogComment("Unable to reference the XmlDataSource.");
                return TestResult.Fail;
            }

            LogComment("XmlDataSource and the testSort element referenced correctly");
            WaitFor(100);
            return TestResult.Pass;
        }

        #endregion

        #region Run Steps

        private TestResult InitialVerify()
        {
            LogComment("Verifing the Initial view");
            FrameworkElement[] flowpanels = Util.FindElements(_testSort, "listItem");
            string[] expected = { "Microsoft C# Language Specification", "Inside C#", "Xml in Action" };

            if (_testSort.ItemsSource == null)
            {
                LogComment("TestSort has no collection");
                return TestResult.Fail;
            }
            return VerifyCollection(flowpanels, expected);
        }

        private TestResult Sort()
        {
            LogComment("Sorting the View");
            ICollectionView cv = _testSort.Items;
            if (cv == null)
            {
                LogComment("ICollectionView is NULL");
                return TestResult.Fail;
            }

            using(cv.DeferRefresh())
            {
                cv.SortDescriptions.Clear();
                cv.SortDescriptions.Add(new SortDescription("Title", System.ComponentModel.ListSortDirection.Ascending));
            }

            return TestResult.Pass;
        }

        private TestResult SortVerify()
        {
            LogComment("Verifing the Sort");
            WaitForPriority(DispatcherPriority.Background);
            WaitFor(100);
            FrameworkElement[] flowpanels = Util.FindElements(_testSort, "listItem");
            string[] expected = { "Inside C#", "Microsoft C# Language Specification", "Xml in Action" };

            return VerifyCollection(flowpanels, expected);
        }

        private TestResult VerifyCollection(FrameworkElement[] flowpanels, string[] expected)
        {
            if (flowpanels.Length != expected.Length)
            {
                LogComment("Expected " + expected.Length + " flowpanels, actual " + flowpanels.Length);
                return TestResult.Fail;
            }
            TextBlock t;

            for (int i = 0; i < expected.Length; i++)
            {
                t = ((StackPanel)flowpanels[i]).Children[0] as TextBlock;
                if (t == null)
                {
                    LogComment("Text was null");
                    return TestResult.Fail;
                }
                if (t.Text != expected[i])
                {
                    LogComment("Text was '" + t.Text + "', expected '" + expected[i] + "' ");
                    return TestResult.Fail;
                }
            }
            LogComment("All Texts had expected values");
            return TestResult.Pass;
        }


        #endregion

        #region Helper Functions



        #endregion

    }
}

