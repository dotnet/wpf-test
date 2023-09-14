// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

////////////////////////////////////////////////////////////////////////////////////
// Description:  Testing Overlapping of spanned cells. If a cell is likely to
//				 overlapp then they need to move to the next available free space.
//
// Verification: property dump
// Created by:	Microsoft
////////////////////////////////////////////////////////////////////////////////////

using System.Windows.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Documents;
using System.Windows.Controls;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;
using Microsoft.Test.Layout;
using Microsoft.Test.Layout.PropertyDump;
using Microsoft.Test.Logging;

namespace Microsoft.Test.FlowLayout
{
    /// <summary>
    /// <area>Viewer.FlowDocumentPageViewer</area>
    /// <owner>Microsoft</owner>
    /// <priority>2</priority>
    /// <description>
    /// Testing Table columnspan and rowspan.
    /// </description>
    /// </summary>
    [Test(2, "Table", "ColRowSpan", MethodName="Run", Variables="VscanMasterPath=FeatureTests\\FlowLayout\\Masters\\VSCAN")]
    public class ColRowSpan : AvalonTest
    {       
        private Window _win;

        public ColRowSpan()
            : base()
        {
            InitializeSteps += new TestStep(Initialize);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(VerifyTest);
        }

        #region Test Steps
        
        /// <summary>
        /// Initialize: Setup the test
        /// </summary>
        /// <returns>TestResult.Pass;</returns>
        private TestResult Initialize()
        {
            _win = new Window();
           
            Status("Initialize");
            Border border = CommonFunctionality.CreateBorder(Brushes.Cornsilk, new Thickness(1), Brushes.Black);
            FlowDocumentScrollViewer eRoot = new FlowDocumentScrollViewer();
            eRoot.Document = new FlowDocument();
            eRoot.VerticalAlignment = VerticalAlignment.Top;
            eRoot.HorizontalAlignment = HorizontalAlignment.Left;
            eRoot.Width = 600;
            eRoot.Height = 400;
            eRoot.ClipToBounds = true;
            BasicTable table = new BasicTable();

            table.Background = Brushes.Salmon;
            table.CreateColumn(new GridLength(100), Brushes.LightGreen);
            table.CreateColumn(new GridLength(100), Brushes.LightPink);
            table.CreateColumn(new GridLength(100), Brushes.LightSkyBlue);

            TableRowGroup body = table.CreateBody();
            TableRow row;

            row = table.CreateRow(body);
            table.CreateCell(row, 1, 1, "CS:1 RS:1");
            table.CreateCell(row, 1, 2, "CS:1 RS:2");
            table.CreateCell(row, 2, 1, "CS:2 RS:1");
            table.CreateCell(row, 3, 3, "CS:3 RS:3 ");
            row = table.CreateRow(body);
            table.CreateCell(row, 1, 1, "CS:1 RS:1");
            table.CreateCell(row, 1, 3, "CS:1 RS:3");
            table.CreateCell(row, 3, 1, "CS:3 RS:1");
            row = table.CreateRow(body);
            table.CreateCell(row, 1, 2, "CS:1 RS:2");
            table.CreateCell(row, 5, 3, "CS:5 RS:3");

            eRoot.Document.Blocks.Add(table.Tbl);
            border.Child = eRoot;

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
            _win.Top = 0;
            _win.Left = 0;
            _win.SizeToContent = SizeToContent.WidthAndHeight;
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
        /// VerifyTest: Verifies the test result
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
