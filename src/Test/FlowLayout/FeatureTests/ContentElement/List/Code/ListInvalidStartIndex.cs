// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description : Test List StartIndex property for invalid values (non positive)
//
// Verification: Basic API validation.  Visual verification is not used.
// Created by:  Microsoft
//////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Threading;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;

namespace Microsoft.Test.FlowLayout
{
    /// <summary>  
    /// Testing commands in FlowDocumentPageViewer.   
    /// </summary>
    [Test(2, "List", "ListInvalidStartIndexTest", MethodName = "Run")]
    public class ListInvalidStartIndexTest : AvalonTest
    {
        private Window _w;
        private int _inputId;
        private List _invalidList0;
        private List _invalidListNeg;

        [Variation(1)]
        [Variation(2)]
        public ListInvalidStartIndexTest(int testId)
            : base()
        {
            _inputId = testId;
            InitializeSteps += new TestStep(Initialize);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(RunTests);
        }

        /// <summary>
        /// Initialize: Setup the test
        /// </summary>
        /// <returns>TestResult.Pass;</returns>
        private TestResult Initialize()
        {
            _w = new Window();

            Status("Initialize");
            FlowDocumentScrollViewer fdsv = new FlowDocumentScrollViewer();
            FlowDocument eRoot = fdsv.Document = new FlowDocument();

            Paragraph liPara1 = new Paragraph(new Run("List Item 1"));
            Paragraph liPara2 = new Paragraph(new Run("List Item 2"));

            ListItem li1 = new ListItem();
            li1.Blocks.Add(liPara1);

            ListItem li2 = new ListItem();
            li2.Blocks.Add(liPara2);

            _invalidList0 = new List();
            _invalidList0.ListItems.Add(li1);
            _invalidList0.ListItems.Add(li2);

            Paragraph liPara3 = new Paragraph(new Run("List Item 3"));
            Paragraph liPara4 = new Paragraph(new Run("List Item 4"));

            ListItem li3 = new ListItem();
            li3.Blocks.Add(liPara3);

            ListItem li4 = new ListItem();
            li4.Blocks.Add(liPara4);

            _invalidListNeg = new List();
            _invalidListNeg.ListItems.Add(li3);
            _invalidListNeg.ListItems.Add(li4);

            eRoot.Blocks.Add(_invalidList0);
            eRoot.Blocks.Add(_invalidListNeg);

            _w.Content = fdsv;
            _w.Width = 800;
            _w.Height = 600;
            _w.Left = 0;
            _w.Top = 0;
            _w.Topmost = true;
            _w.Show();
            WaitForPriority(DispatcherPriority.ApplicationIdle);

            return TestResult.Pass;
        }

        private TestResult CleanUp()
        {
            _w.Close();
            return TestResult.Pass;
        }

        /// <summary>
        /// RunTests: Runs a set of tests based on the input variation.
        /// </summary>
        /// <returns>TestResult</returns>
        private TestResult RunTests()
        {
            switch (_inputId)
            {
                case 1: Log.Result = TryInvalidStartIndexValue(_invalidList0, 0);
                    break;
                case 2: Log.Result = TryInvalidStartIndexValue(_invalidListNeg, -1);
                    break;
            }

            return TestResult.Pass;
        }

        private TestResult TryInvalidStartIndexValue(List testList, int startIndex)
        {
            try
            {
                testList.StartIndex = startIndex;
                LogComment("Failed to trigger exception!  StartIndex of " + startIndex + " should not work.");
                return TestResult.Fail;
            }
            catch (System.ArgumentException e)
            {
                LogComment("Recieved expected exception!" + e.Message);
                return TestResult.Pass;
            }
        }
    }
}
