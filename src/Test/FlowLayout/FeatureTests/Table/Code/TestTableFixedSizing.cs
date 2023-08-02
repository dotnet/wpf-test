// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*
    Test to prevent regression of Regression_Bug11
    Table with Body and Row but no cells throws PTS exception in MeasureOverride
*/
using System.Windows.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Documents;
using System.Windows.Controls;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;
using Microsoft.Test.Layout.PropertyDump;
using Microsoft.Test.Layout;
using Microsoft.Test.Logging;

namespace Microsoft.Test.FlowLayout
{
    /// <summary>
    /// <area>Table</area>
    /// <owner>Microsoft</owner>
    /// <priority>0</priority>
    /// <description>
    /// Testing Table fixed sizing.
    /// </description>
    /// </summary>
    [Test(2, "Table", "TableFixedSizing", MethodName="Run", Variables="VscanMasterPath=FeatureTests\\FlowLayout\\Masters\\VSCAN")]
    public class TableFixedSizing : AvalonTest
    {
        #region Test case members
        private Window _win;
        #endregion

        #region Constructor

        public TableFixedSizing()
            : base()
        {
            InitializeSteps += new TestStep(Initialize);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(VerifyTest);
        }

        #endregion

        #region Test Steps

        /// <summary>
        /// Initialize: Setup the test
        /// </summary>
        /// <returns>TestResult.Pass;</returns>
        private TestResult Initialize()
        {
            Status("Initialize");
            Border border = CommonFunctionality.CreateBorder(Brushes.Cornsilk, new Thickness(1), Brushes.Black);
            FlowDocumentScrollViewer eRoot = new FlowDocumentScrollViewer();
            eRoot.Document = new FlowDocument();
            eRoot.HorizontalAlignment = HorizontalAlignment.Left;
            eRoot.VerticalAlignment = VerticalAlignment.Top;
            BasicTable table = new BasicTable();
            eRoot.Width = 600;
            eRoot.Height = 600;
            eRoot.ClipToBounds = true;
            table.CreateColumn();
            table.CreateColumn();
            table.CreateColumn();
            table.CreateColumn();

            TableRow row;
            TableRowGroup header = table.CreateBody();

            row = table.CreateRow(header);
            table.CreateCell(row, "Category");
            table.CreateCell(row, "Description");
            table.CreateCell(row, "Downloads");
            table.CreateCell(row, "Release Date");
            table.CreateCell(row, "Channel");

            TableRowGroup body = table.CreateBody();

            row = table.CreateRow(body);
            table.CreateCell(row, "Introduction");
            table.CreateCell(row, "abab abab abab abab");
            table.CreateCell(row, "cdcd cdcd cdcd cdcd cdcd cdcd cdcd cdcd cdcdcdcdcd cdcdcd cdcdcdcdcdcd");
            table.CreateCell(row, "May 1  May1  May 10  May 15  May 15 ");
            table.CreateCell(row, "x  x  x  x");
            row = table.CreateRow(body);
            table.CreateCell(row, "Best Practices");
            table.CreateCell(row, "efefefef efef efefefefefef efefe ef");
            table.CreateCell(row, "ghghghg ghghg");
            table.CreateCell(row, "May 1  May1  May 10  May 15  May 15 ");
            table.CreateCell(row, "");

            TableRowGroup footer = table.CreateBody();

            row = table.CreateRow(footer);
            table.CreateCell(row, "Footer");
            table.CreateCell(row, "Footer");
            table.CreateCell(row, "Footer");

            eRoot.Document.Blocks.Add(table.Tbl);            
            border.Child = eRoot;
            _win = new Window();
            _win.Content = border;
            if (_win.Content is FrameworkElement)
            {
                // Setting Window.Content size to ensure same size of root element over all themes.  
                // Different themes have different sized window chrome which will cause property dump 
                // and vscan failures even though the rest of the content is the same.
                // 784x564 is the content size of a 800x600 window in Aero theme.
                ((FrameworkElement)_win.Content).Height = 564;
                ((FrameworkElement)_win.Content).Width = 784;
            }
            _win.SizeToContent = SizeToContent.WidthAndHeight;            
            _win.Top = 0;
            _win.Left = 0;
            _win.Resources.MergedDictionaries.Add(GenericStyles.LoadAllStyles());
            _win.Show();

            WaitForPriority(DispatcherPriority.ApplicationIdle);
            return TestResult.Pass;
        }

        private TestResult CleanUp()
        {
            _win.Close();
            return TestResult.Pass;
        }

        /// <summary>
        /// VerifyTest: Verify the test.
        /// </summary>
        /// <returns>TestResult</returns>
        private TestResult VerifyTest()
        {
            WaitForPriority(DispatcherPriority.ApplicationIdle);
            Status("Property Dump Verification");
            try
            {
                PropertyDumpHelper helper = new PropertyDumpHelper((Visual)_win.Content);
                if (helper.CompareLogShow(new Arguments(this)))
                {
                    return TestResult.Pass;
                }
                else
                {
                    return TestResult.Fail;
                }
            }
            catch (System.Xml.XmlException)
            {
                return TestResult.Ignore;
            }
        }
        #endregion
    }
}
