// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

////////////////////////////////////////////////////////////////////////////////////
// Description:  Testing Width , minWidth and maxWidth property set table on resizing the window.
//
// Verification: Testcase 1: Minwidth = 120; Width = 200; Maxwidth=1000. Width should win and on resizing the window
//               computed width size will remain 200px.
//
//               TestCase 2: Minwidth = 400; Width = 100; Maxwidth=1000. Width should win and on resizing the window
//               computed width size will remain 100px.
//
//               TestCase 3: Minwidth = 300; Width = 600; Maxwidth=100. Width should win and on resizing the window
//               computed width size will remain 600px.
//
//               TestCase 4: Minwidth = 250; Width = 0; Maxwidth=1000. Width is 0 so MinWidth is applied and as the
//               window is resized the table resize with the boundary of minwidth and maxwidth i.e between 250 and 1000.
//
//
// Created by:  Microsoft
////////////////////////////////////////////////////////////////////////////////////
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
    /// Testing Table resize width.   
    /// </summary>
    [Test(0, "Table", "ResizeTableWidth", MethodName="Run")]
    public class ResizeTableWidth : AvalonTest
    {
        #region Test case members

        private Window _win;
        private int _testId;
        private FlowDocumentScrollViewer _eRoot;        
        private BasicTable _table;        
        private string _inputString;

        #endregion

        [Variation(0, "Test1")]
        [Variation(1, "Test2", Priority = 3)]
        [Variation(2, "Test3", Priority = 3)]
        [Variation(3, "Test4", Priority = 3)]
        [Variation(4, "Test5", Priority = 3)]        

        #region Constructor
    
        public ResizeTableWidth(int testValue, string testString)
            : base()
        {
            _testId = testValue;
            _inputString = testString;
            InitializeSteps += new TestStep(Initialize);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(RunTests);
            RunSteps += new TestStep(VerifyTest);
        }

        #endregion

        #region Test Steps     
        /// <summary>
        /// Initialize: Setup the test
        /// </summary>
        /// <returns>TestResult</returns>
        private TestResult Initialize()
        {
            _win = new Window();
                       
            Status("Initialize");
            Grid TopRoot = new Grid();
            _eRoot = new FlowDocumentScrollViewer();
            _eRoot.Document = new FlowDocument();
            _table = new BasicTable();
            _table.Background = Brushes.Salmon;
            _eRoot.Height = 300;
            _table.CreateColumn(Brushes.LightBlue);
            _table.CreateColumn(Brushes.LightGreen);
            _table.CreateColumn(Brushes.LightPink);

            TableRow row;
            TableRowGroup body = _table.CreateBody();

            row = _table.CreateRow(body);
            _table.CreateCell(row, "Cell 1");
            _table.CreateCell(row, "Cell 2");
            _table.CreateCell(row, "Cell 3");
            row = _table.CreateRow(body);
            _table.CreateCell(row, "Cell 4");
            _table.CreateCell(row, "Cell 5");
            _table.CreateCell(row, "Cell 6");
            row = _table.CreateRow(body);
            _table.CreateCell(row, "Cell 7");
            _table.CreateCell(row, "Cell 8");
            _table.CreateCell(row, "Cell 9");

            TopRoot.Children.Add(_eRoot);
            _eRoot.Document.Blocks.Add(_table.Tbl);
            DynamicChanges(_testId);
            _win.Content = TopRoot;           
            ((FrameworkElement)_win.Content).Width = 1000;
            ((FrameworkElement)_win.Content).Height = 1000;
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
        /// RunTests: Runs a set of tests based on the input variation.
        /// </summary>
        /// <returns>TestResult</returns>
        private TestResult RunTests()
        {
            WaitForPriority(DispatcherPriority.SystemIdle);
            
            LogComment("....Resizing the window.....");
            for (int i = 0; i < 6; i++)
            {
                switch (i)
                {
                    case 0:
                        {                          
                            ((FrameworkElement)_win.Content).Width = 550;
                            ((FrameworkElement)_win.Content).Height = 350;  
                            LogComment("Table Calculated Width = " + _eRoot.RenderSize.Width);
                            break;
                        }

                    case 1:
                        {                           
                            ((FrameworkElement)_win.Content).Width = 245;
                            ((FrameworkElement)_win.Content).Height = 750;  
                            LogComment("Table Calculated Width = " + _eRoot.RenderSize.Width);
                            break;
                        }

                    case 2:
                        {                           
                            ((FrameworkElement)_win.Content).Width = 100;
                            ((FrameworkElement)_win.Content).Height = 100;  
                            LogComment("Table Calculated Width = " + _eRoot.RenderSize.Width);
                            break;
                        }

                    case 3:
                        {                           
                            ((FrameworkElement)_win.Content).Width = 1500;
                            ((FrameworkElement)_win.Content).Height = 350;  
                            LogComment("Table Calculated Width = " + _eRoot.RenderSize.Width);
                            break;
                        }

                    case 4:
                        {                            
                            ((FrameworkElement)_win.Content).Width = 1002;                            
                            LogComment("Table Calculated Width = " + _eRoot.RenderSize.Width);
                            break;
                        }
                    case 5:
                        {                         
                            ((FrameworkElement)_win.Content).Width = 800;
                            ((FrameworkElement)_win.Content).Height = 600;  
                            LogComment("Table Calculated Width = " + _eRoot.RenderSize.Width);
                            break;
                        }
                    default:
                        {                      
                            ((FrameworkElement)_win.Content).Width = 1000;
                            ((FrameworkElement)_win.Content).Height = 1000;  
                            LogComment("Table Calculated Width = " + _eRoot.RenderSize.Width);
                            break;
                        }
                }
            }

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

        private void DynamicChanges(int testId)
        {                       
            switch (testId)
            {
                case 1:
                    _eRoot.MinHeight = 120;
                    _eRoot.Height = 200;
                    _eRoot.MaxHeight = 1000;
                    break;
                case 2:
                    _eRoot.MinHeight = 400;
                    _eRoot.Height = 100;
                    _eRoot.MaxHeight = 1000;
                    break;
                case 3:
                    _eRoot.MinHeight = 300;
                    _eRoot.Height = 600;
                    _eRoot.MaxHeight = 100;
                    break;
                case 4:
                    _eRoot.MinHeight = 250;
                    _eRoot.MaxHeight = 800;
                    break;
                default:
                    _eRoot.MinHeight = 0;
                    _eRoot.MaxHeight = 0;
                    break;
            }
        }
    }
}
