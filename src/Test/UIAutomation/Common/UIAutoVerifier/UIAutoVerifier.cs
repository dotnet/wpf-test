// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//

#region Using Statements

using System;
using System.Text;
using System.Windows.Automation;
using System.Windows.Automation.Text;
using System.Windows.Controls;
using System.Diagnostics;
using System.Threading;

using Microsoft.Test.WindowsUIAutomation;
using Microsoft.Test.WindowsUIAutomation.Logging;
using Microsoft.Test.WindowsUIAutomation.TestManager;


#endregion

namespace WPF.Test.UIAutomation
{
    /// <summary>
    /// Driver for Accessibility test cases
    /// It creates an application and find out the first instance of a specific class.
    /// And then run all accessibility test cases against the instance.
    /// 
    /// Usage: UIAutoVerifier priority classname application [arguments]
    /// priority: you can choose from 0, 1, and 2, 3;
    /// </summary>
    public class UIAutoVerifier
    {
        #region Private Fields
                
        private static TestPriorities s_priority;
        

        #endregion

        public int RunVerifierTest(string pri, string id, string fileName, string otherArgs)
        {
            string[] args = { pri, id, fileName, otherArgs };
            return Main(args);
        }

        public static int Main(string[] args)
        {
            UIVerifyLogger.SetLoggerType(UIVerifyLogger.PiperLogger);

            if (args.Length < 3)
            {
                ShowUsage();
                return -1;
            }

            //TestPriorities priority;
            switch (args[0])
            {
                case "0":
                    s_priority = TestPriorities.Pri0;
                    break;
                case "1":
                    s_priority = TestPriorities.Pri1;
                    break;
                case "2":
                    s_priority = TestPriorities.Pri2;
                    break;
                case "3":
                    s_priority = TestPriorities.Pri3;
                    break;
                default:
                    UIVerifyLogger.LogComment("You should choose priority from 0, 1, 2, 3");
                    return -1;
            }

            string id = args[1];
            string fileName = args[2];
            string arguments;
            {
                // build the arguments
                StringBuilder strb = new StringBuilder();
                for (int i = 3; i < args.Length; ++i)
                {
                    strb.Append(args[i] + " ");
                }
                arguments = strb.ToString();
            }

            Process process = null;
            try
            {
                // start a process
                process = new Process();
                process.StartInfo.FileName = fileName;
                process.StartInfo.Arguments = arguments;
                process.Start();

                // find the process main window handle
                const int MAXTIME = 30000; // maximum time to wait
                const int TIMEWAIT = 100; // polling time interval
                int runningTime = 0;                
                // 
                Thread.Sleep(MAXTIME);

                while (process.MainWindowHandle.Equals(IntPtr.Zero))
                {
                    if (runningTime > MAXTIME)
                    {
                        UIVerifyLogger.LogComment("Cannot not find " + process.StartInfo.FileName);
                        return -1;
                    }

                    Thread.Sleep(TIMEWAIT);
                    runningTime += TIMEWAIT;

                    process.Refresh();
                }

                IntPtr handle = process.MainWindowHandle;

                // find the root window
                AutomationElement root = AutomationElement.FromHandle(handle);
                if (root == null)
                {
                    UIVerifyLogger.LogComment("Could not convert root window handle to AutomationElement");
                    return -1;
                }

                AutomationElement element = root.FindFirst(TreeScope.Descendants | TreeScope.Children,
                    new PropertyCondition(AutomationElement.AutomationIdProperty, id));
                if (element == null)
                {
                    UIVerifyLogger.LogComment("Could not find instances of id " + id);
                    return -1;
                }

                bool res = true;
                if (id == "VirtualTestObject")
                {
                    res = ItemContainerTests(element);
                }
                else
                {
                    if (s_priority == TestPriorities.Pri0)
                    {
                        res = BasicTest(element);
                    }

                    if (CheckWPFDataGridElement(element))
                    {
                        res = WPFDataGrid(element);
                    }
                    else
                    {
                        if (!TestRuns.RunAllTests(element, true, s_priority, TestCaseType.Generic, false, false, null))
                        {
                            res = false;
                        }
                    }
                }
                return res ? 0 : -1;
            }
            catch (Exception e)
            {
                UIVerifyLogger.LogComment("" + e);
                return -1;
            }
            finally
            {
                if (process != null)
                    process.Kill();
                UIVerifyLogger.CloseLog();
            }
        }

        private static void ShowUsage()
        {
            UIVerifyLogger.LogComment("Usage: UIAutoVerifier priority id application [arguments]");
        }

        #region OOB Tests
        /// <summary>
        /// test what is not covered in the test framework 
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        private static bool BasicTest(AutomationElement element)
        {
            string className = (string)element.GetCurrentPropertyValue(AutomationElement.ClassNameProperty);
            UIVerifyLogger.LogComment(string.Format("In BasicTest: the actualClassName is - {0}, the AutomationId is - {1}, and the ControlType is - {2}", className, element.Current.AutomationId, ((ControlType)element.GetCurrentPropertyValue(AutomationElement.ControlTypeProperty)).LocalizedControlType));
            className = (string)element.GetCurrentPropertyValue(AutomationElement.ClassNameProperty);

            if ("StatusBar" == className)
                return RunTest(StatusBarTest, element);
            if ("ListView" == className)
                return RunTest(GridViewTest, element);
            if ("Document" == className)
            {
                AutomationElement ae = TreeWalker.ControlViewWalker.GetFirstChild(element);
                if (ae == null)
                {
                    if (CheckTableTest(element))
                    {
                        return RunTest(FlowDocumentTableTest, element);
                    }
                    else
                    {
                        // just log and return
                        UIVerifyLogger.LogComment("************************ This is a FlowDocument without Table ************************");
                    }
                }
                else
                {
                    // just log and return
                    UIVerifyLogger.LogComment("************************ This is a Document with a first child ************************");
                }
            }
            return true;
        }

        /// <summary>
        ///Entry for the OOB ItemContainer pattern tests
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        private static bool ItemContainerTests(AutomationElement element)
        {
            bool result = true;

            if (s_priority == TestPriorities.Pri0)
            {
                if (!RunTest(ItemContainerPatternFindByName, element))
                {
                    result = false;
                }
                if (!RunTest(ItemContainerPatternFindByAutomationId, element))
                {
                    result = false;
                }
                if (!RunTest(ItemContainerPatternFindAll, element))
                {
                    result = false;
                }
            }
            else
            {
                if (!RunTest(ItemContainerPatternFindBySelection, element))
                {
                    result = false;
                }
                if (!RunTest(ItemContainerPatternFindByControlType, element))
                {
                    result = false;
                }
            }
            return result;
        }
        /// <summary>
        /// Tests FindItemByProperty by passing NameProperty to the API
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        private static bool ItemContainerPatternFindByName(AutomationElement element)
        {
            string testValue = "LBI0075";

            if (!CheckItemContainerPattternSupported(element))
            {
                return (CheckUiaCoreOld());
            }
            ItemContainerPattern itemContainerPattern = element.GetCurrentPattern(ItemContainerPattern.Pattern) as ItemContainerPattern;

            AutomationElement targetElement = null;
            targetElement = itemContainerPattern.FindItemByProperty(null, AutomationElement.NameProperty, testValue);

            if (!DeVirtualizeItemAndVerify(targetElement))
            {
                return false;
            }
            if ((targetElement.Current.Name.ToString()) != testValue)
            {
                UIVerifyLogger.LogComment("Realized element is different from what we expected");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Tests FindItemByProperty with selected item in query
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        private static bool ItemContainerPatternFindBySelection(AutomationElement element)
        {
            string testValue = "LBI0100";
            string testValuePlusOne = "LBI0101";

            if (!CheckItemContainerPattternSupported(element))
            {
                return (CheckUiaCoreOld());
            }
            ItemContainerPattern itemContainerPattern = element.GetCurrentPattern(ItemContainerPattern.Pattern) as ItemContainerPattern;

            AutomationElement targetElement = null;
            targetElement = itemContainerPattern.FindItemByProperty(null, AutomationElement.NameProperty, testValue);
            if (!DeVirtualizeItemAndVerify(targetElement))
            {
                return false;
            }
            SelectionItemPattern selectionItemPattern;
            selectionItemPattern = targetElement.GetCurrentPattern(SelectionItemPattern.Pattern) as SelectionItemPattern;
            try
            {
                selectionItemPattern.AddToSelection();
            }
            catch
            {
                UIVerifyLogger.LogComment("Failed to select item");
                return false;
            }
            //Navigating to the next item to verify selected item can be found even before the current item with null passed
            itemContainerPattern.FindItemByProperty(null, AutomationElement.NameProperty, testValuePlusOne);
            targetElement = itemContainerPattern.FindItemByProperty(null, SelectionItemPatternIdentifiers.IsSelectedProperty, true);

            if ((targetElement.Current.Name.ToString()) != testValue)
            {
                UIVerifyLogger.LogComment("Element found is different from what we expected");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Tests FindItemByProperty by passing AutomationID of the element
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        private static bool ItemContainerPatternFindByAutomationId(AutomationElement element)
        {
            string testValue = "Item175";

            if (!CheckItemContainerPattternSupported(element))
            {
                return (CheckUiaCoreOld());
            }
            
            ItemContainerPattern itemContainerPattern = element.GetCurrentPattern(ItemContainerPattern.Pattern) as ItemContainerPattern;

            AutomationElement targetElement = null;
            targetElement = itemContainerPattern.FindItemByProperty(null, AutomationElement.AutomationIdProperty, testValue);
            if (!DeVirtualizeItem(targetElement))
            {
                return false;
            }

            if ((targetElement.Current.AutomationId.ToString()) != testValue)
            {
                UIVerifyLogger.LogComment("Realized element is different from what we expected");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Tests FindItemByProperty by searching based on control type of the child item
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        private static bool ItemContainerPatternFindByControlType(AutomationElement element)
        {
            string testValue = "LBI0220";
            string testValuePlusOne = "LBI0221";
            if (!CheckItemContainerPattternSupported(element))
            {
                return (CheckUiaCoreOld());
            } 
            ItemContainerPattern itemContainerPattern = element.GetCurrentPattern(ItemContainerPattern.Pattern) as ItemContainerPattern;

            AutomationElement targetElement = null;
            targetElement = itemContainerPattern.FindItemByProperty(null, AutomationElement.NameProperty, testValue);
            targetElement = itemContainerPattern.FindItemByProperty(targetElement, AutomationElement.ControlTypeProperty, targetElement.Current.ControlType);
            if (!DeVirtualizeItemAndVerify(targetElement))
            {
                return false;
            }

            if ((targetElement.Current.Name.ToString()) != testValuePlusOne)
            {
                UIVerifyLogger.LogComment("Realized element is different from what we expected");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Tests FindItemByProperty by finding every single child in a row after given start element
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        private static bool ItemContainerPatternFindAll(AutomationElement element)
        {
            string testValue = "LBI0275";
            if (!CheckItemContainerPattternSupported(element))
            {
                return (CheckUiaCoreOld());
            }
            ItemContainerPattern itemContainerPattern = element.GetCurrentPattern(ItemContainerPattern.Pattern) as ItemContainerPattern;

            AutomationElement targetElement = null;
            targetElement = itemContainerPattern.FindItemByProperty(null, AutomationElement.NameProperty, testValue);
            targetElement = itemContainerPattern.FindItemByProperty(targetElement, null, "this parameter is irrelevant");
            if (!DeVirtualizeItemAndVerify(targetElement))
            {
                return false;
            }
            
            VirtualizedItemPattern virtualizedItemPattern;
            
            while (targetElement != null) 
            {

                try
                {
                    virtualizedItemPattern = targetElement.GetCurrentPattern(VirtualizedItemPattern.Pattern) as VirtualizedItemPattern;
                
                    if (!DeVirtualizeItem(targetElement))
                    {
                        return false;
                    }
                }
                catch
                {}
                targetElement = itemContainerPattern.FindItemByProperty(targetElement, null, "this parameter is irrelevant");
            } 
  
            return true;
        }

        /// <summary>
        /// test method call back
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        private delegate bool TestMethod(AutomationElement element);

        /// <summary>
        /// create a test session
        /// </summary>
        /// <param name="method"></param>
        /// <param name="element"></param>
        /// <returns></returns>
        private static bool RunTest(TestMethod method, AutomationElement element)
        {
            // start a test case, use its method's name as the test name
            Logger.StartTest(method.Method.Name);

            // run the test case
            try
            {
                bool res = method(element);
                if (res)
                    Logger.LogPass();
                else
                    Logger.LogError("Test Case failed");
                return res;
            }
            catch (Exception e)
            {
                Logger.LogError(e, true);
                return false;
            }
            finally
            {
                // end the test
                Logger.EndTest();
            }
        }

        /// <summary>
        /// WPF DataGrid Tests Starting Point 
        /// NOTE - the UI code is specifically structured so that all related components can be tested
        ///        So don't change the UI code unless you know what you change
        /// </summary>
        /// <param name="element">the AE to test against</param>
        /// <returns>true if pass, else fail</returns>
        private static bool WPFDataGrid(AutomationElement element)
        {
            bool result = true;
            if (element == null)
                return false;

            // a. DG
            RunAllTestsByElement(element, "DataGrid tests failed!");

            // b. header 
            AutomationElement columnheaderpresenter = FindFirstChildByClassName(element, "DataGridColumnHeadersPresenter");
            RunAllTestsByElement(columnheaderpresenter, "ColumnHeaderPresenter tests failed!");

            // c. header item under b
            AutomationElement columnheader = FindFirstChildByClassName(columnheaderpresenter, "DataGridColumnHeader");
            RunAllTestsByElement(columnheader, "ColumnHeader tests failed!");

            // 

            // d. test the custom
            AutomationElement rowspresenter = GetTheNextSibling(columnheaderpresenter);
            RunAllTestsByElement(rowspresenter, "RowsPresenter tests failed!");

            // e. test the dataitem under d.
            AutomationElement dataitem = FindFirstChildByClassName(rowspresenter, "DataGridRow");
            RunAllTestsByElement(dataitem, "DataItem tests failed!");

            // f. test the HeaderItem (rowheader) under e
            AutomationElement rowheader = FindFirstChildByClassName(dataitem, "DataGridRowHeader");
            RunAllTestsByElement(rowheader, "RowHeader tests failed!");

            // g. test the Custom (cell) under e
            AutomationElement cell = GetTheNextSibling(rowheader);
            RunAllTestsByElement(cell, "Cell tests failed!");

            // h. test the Custom (detailspresenter) under e
            AutomationElement detail = GetTheLastChild(TreeWalker.ControlViewWalker.GetParent(cell));
            if (string.Equals("DataGridDetailsPresenter", (string)detail.GetCurrentPropertyValue(AutomationElement.ClassNameProperty), StringComparison.InvariantCulture))
            {
                RunAllTestsByElement(detail, "Details tests failed!");
            }

            // headers - 
            if (!VerifyTablePattern(element))
            {
                UIVerifyLogger.LogComment("TablePattern test for headers failed!");
                result = false;
            }

            return result;
        }

        private static bool VerifyTablePattern(AutomationElement element)
        {
            if (element == null)
            {
                UIVerifyLogger.LogComment("The element should not be null");
                return false;
            }

            TablePattern table = (TablePattern)element.GetCurrentPattern(TablePattern.Pattern);
            int column = table.Current.ColumnCount;
            int row = table.Current.RowCount;
            UIVerifyLogger.LogComment(string.Format("The current counts per TablePattern - Column: {0} Row: {1}", column, row));

            AutomationElement[] rowHeaders = table.Current.GetRowHeaders();
            AutomationElement[] columnHeaders = table.Current.GetColumnHeaders();

            foreach (AutomationElement rowHeader in rowHeaders)
            {
                if (rowHeader.Current.ControlType.LocalizedControlType != "header item")
                {
                    UIVerifyLogger.LogComment("The row header type is incorrect");
                    return false;
                }

                if (string.IsNullOrEmpty(rowHeader.Current.Name))
                {
                    UIVerifyLogger.LogComment("The row header content is incorrect");
                    return false;
                }
            }

            foreach (AutomationElement columnHeader in columnHeaders)
            {
                if (columnHeader.Current.ControlType.LocalizedControlType != "header item")
                {
                    UIVerifyLogger.LogComment("The column header type is incorrect");
                    return false;
                }

                if (string.IsNullOrEmpty(columnHeader.Current.Name))
                {
                    UIVerifyLogger.LogComment("The column header content should not be empty");
                    return false;
                }
            }
            return true;
        }

        private static bool RunAllTestsByElement(AutomationElement element, string msg)
        {
            bool resultLoc = true;
            if (element == null)
            {
                Logger.LogComment(string.Format("The element {0} cannot be null", element.ToString()));
                resultLoc = false;
            }
            if (!TestRuns.RunAllTests(element, true, s_priority, TestCaseType.Generic, false, false, null))
            {
                Logger.LogComment(msg);
                resultLoc = false;
            }
            return resultLoc;
        }

        /// <summary>
        /// test status bar's properties and its children's properties.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        private static bool StatusBarTest(AutomationElement element)
        {
            // verify StatusBar's ControlType=StatusBar
            if (!VerifyControlType(element, ControlType.StatusBar))
                return false;

            // verify StatusBar's IsContent=True
            bool isContentElement = (bool)element.GetCurrentPropertyValue(AutomationElement.IsContentElementProperty);
            UIVerifyLogger.LogComment("IsContentElement: Exp.[{0}] Act.[{1}]", true, isContentElement);
            if (!isContentElement)
                return false;

            // iterate its children and verify its items' ControlType=Text if they are StatusBarItem
            for (AutomationElement child = TreeWalker.ControlViewWalker.GetFirstChild(element);
                child != null; child = TreeWalker.ControlViewWalker.GetNextSibling(child))
            {
                if (CheckClassName(child, "StatusBarItem"))
                {
                    if (!VerifyControlType(child, ControlType.Text))
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Check if there is Table element 
        /// </summary>
        /// <param name="element">the parent element</param>
        /// <returns>true if Table, else not</returns>
        private static bool CheckTableTest(AutomationElement element)
        {
            TextPattern textPattern = element.GetCurrentPattern(TextPattern.Pattern) as TextPattern;
            AutomationElement[] children = textPattern.DocumentRange.GetChildren();
            if (children.Length == 0)
            {
                UIVerifyLogger.LogComment("************************ In CheckTableTest - no Children! ************************");
                return false;
            }

            AutomationElement aeTable = children[0];
            // verify the control type
            if (!VerifyControlType(aeTable, ControlType.Table))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// test Table control's properties, control type, and patterns 
        /// </summary>
        /// <param name="element">the test object - FlowDocument</param>
        /// <returns></returns>
        private static bool FlowDocumentTableTest(AutomationElement element)
        {
            //get the children of the element
            bool isTable = false;
            AutomationElement aeTable = null;
            TextPattern textPattern = element.GetCurrentPattern(TextPattern.Pattern) as TextPattern;
            AutomationElement[] children = textPattern.DocumentRange.GetChildren();
            if (children.Length > 0)
            {
                isTable = true;
                aeTable = children[0];
            }
            else
            {
                // stop here and go back for proper tests elsewhere - Document/Text/DocumentRange
                aeTable = element;
                return true;
            }

            if (isTable)
            {
                // verify control type, and the support for GridPattern and GridItemPattern
                GridPattern grid = (GridPattern)aeTable.GetCurrentPattern(GridPattern.Pattern);
                int column = grid.Current.ColumnCount;
                int row = grid.Current.RowCount;
                UIVerifyLogger.LogComment(string.Format("The current counts - Column: {0} Row: {1}", column, row));

                for (int i = 0; i < row; i++)
                {
                    for (int j = 0; j < column; j++)
                    {
                        AutomationElement ae = grid.GetItem(i, j);

                        // verify the ControlType w/ LocalizedControlType
                        if (!VerifyControlType(ae, ControlType.Custom)) return false;

                        //verify the GridItemPattern
                        if (!CheckGridItemPattern(ae, j, i)) return false;
                    }
                }
            }

            return true;
        }

        // the tolerance used when comparing two doubles
        private const double TOLERANCE = 0.1;


        /// <summary>
        /// Verify header's InvokePattern
        /// </summary>
        /// <param name="header">the header to eval against</param>
        /// <returns>true if ok, false failed</returns>
        private static bool VerifyHeaderInvokePattern(AutomationElement header)
        {
            // get the InvokePattern 
            InvokePattern ip = header.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;

            try
            {
                ip.Invoke();
            }
            catch (Exception actualException)
            {
                Logger.LogComment("Exception was throw in Invoking a header - {0}", actualException.Message);
                return false;
            }
            Logger.LogComment("InvokePattern.Invoke done for the header without exception. ");
            return true;
        }

        /// <summary>
        /// verify header's TransformPattern
        /// </summary>
        /// <param name="header"></param>
        /// <returns></returns>
        private static bool VerifyHeaderTransformPattern(AutomationElement header)
        {
            // get TransformPattern
            TransformPattern transform = (TransformPattern)header.GetCurrentPattern(TransformPattern.Pattern);
            if (!transform.Current.CanResize)
            {
                Logger.LogComment("Header not support resize");
                return false;
            }

            // resize header's size to 20,30
            transform.Resize(20, 30);

            // verify its size is changed to 20,30
            Thread.Sleep(new TimeSpan(0, 0, 1));
            System.Windows.Rect rect = (System.Windows.Rect)header.GetCurrentPropertyValue(AutomationElement.BoundingRectangleProperty);
            if (Math.Abs(rect.Height - 30) > TOLERANCE || Math.Abs(rect.Width - 20) > TOLERANCE)
                return false;

            // verify resize throws exception if its width is negative
            try
            {
                transform.Resize(-1, 2);
                Logger.LogComment("No exception raised when resize parameters are invalid");
                return false;
            }
            catch (ArgumentOutOfRangeException)
            {
            }

            // verify resize throws exception if its height is negative
            try
            {
                transform.Resize(1, -2);
                Logger.LogComment("No exception raised when resize parameters are invalid");
                return false;
            }
            catch (ArgumentOutOfRangeException)
            {
            }

            // verify header does not support rotate
            if (transform.Current.CanRotate)
            {
                Logger.LogComment("Rotate should not be supported on header");
                return false;
            }

            // verify rotate throws exception
            try
            {
                transform.Rotate(90);
                Logger.LogComment("No exception raised when trying to rotate a header");
                return false;
            }
            catch (InvalidOperationException)
            {
            }
            Logger.LogComment("TransformPattern tests done for the header without exception. ");
            return true;
        }

        /// <summary>
        /// test grid view's properties and its header's transfrom pattern
        /// and its cells' properties
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>







        private static bool GridViewTest(AutomationElement element)
        {
            // verify GridView's ControlType is DataGrid
            if (!VerifyControlType(element, ControlType.DataGrid))
                return false;

            // find its header and verify transform pattern against
            AutomationElement header = element.FindFirst(TreeScope.Descendants,
                new PropertyCondition(AutomationElement.ClassNameProperty, "GridViewColumnHeader"));
            if (header == null)
            {
                Logger.LogComment("No header found");
                return false;
            }
            else
            {
                // Verify the ControlType, test InvokePattern and TransformPattern
                if (!VerifyControlType(header, ControlType.HeaderItem))
                    return false;

                if (!VerifyHeaderInvokePattern(header))
                    return false;

                if (!VerifyHeaderTransformPattern(header))
                    return false;
            }

            // iterate all items
            AutomationElement headerRowPresenter = TreeWalker.ControlViewWalker.GetFirstChild(element);
            int rowIndex = 0;
            for (AutomationElement child = TreeWalker.ControlViewWalker.GetNextSibling(headerRowPresenter);
                child != null; child = TreeWalker.ControlViewWalker.GetNextSibling(child))
            {
                // 

                if (child.Current.ControlType == ControlType.Group)
                {
                    

                    for (AutomationElement child2 = TreeWalker.ControlViewWalker.GetFirstChild(child);
                        child2 != null; child2 = TreeWalker.ControlViewWalker.GetNextSibling(child2))
                    {
                        if (!VerifyGridViewCell(child2, rowIndex++))
                            return false;
                    }
                }
                else
                {
                    if (!VerifyGridViewCell(child, rowIndex++))
                        return false;
                }
            }

            // 


            // get the current (column, row)
            GridPattern grid = (GridPattern)element.GetCurrentPattern(GridPattern.Pattern);
            int column = grid.Current.ColumnCount;
            int row = grid.Current.RowCount;
            UIVerifyLogger.LogComment(string.Format("The current counts per GridPattern - Column: {0} Row: {1}", column, row));

            TablePattern table = (TablePattern)element.GetCurrentPattern(TablePattern.Pattern);
            column = table.Current.ColumnCount;
            row = table.Current.RowCount;
            UIVerifyLogger.LogComment(string.Format("The current counts per TablePattern - Column: {0} Row: {1}", column, row));

            //// remove an item
            //AutomationElement listBox = TreeWalker.ControlViewWalker.GetPreviousSibling(element);
            //AutomationElement listBoxItem = TreeWalker.ControlViewWalker.GetFirstChild(listBox);
            //SelectionItemPattern selection = (SelectionItemPattern)listBoxItem.GetCurrentPattern(SelectionItemPattern.Pattern);
            //selection.RemoveFromSelection();

            //UIVerifyLogger.LogComment(string.Format("After items changed: Row: Exp: {0} Act: {1}", row - 1, grid.Current.RowCount));
            //if (grid.Current.RowCount != row - 1)
            //    return false;

            //// add a column
            //AutomationElement addColumnButton = TreeWalker.ControlViewWalker.GetPreviousSibling(listBox);
            //InvokePattern invoke = (InvokePattern)addColumnButton.GetCurrentPattern(InvokePattern.Pattern);
            //invoke.Invoke();

            //// verify the column count gets changed
            //UIVerifyLogger.LogComment(string.Format("After columns changed: Column: Exp: {0} Act: {1}", column + 1, grid.Current.ColumnCount));
            //if (grid.Current.ColumnCount != column + 1)
            //    return false;

            return true;
        }

        #endregion

        #region Local Helpers and Verifiers
        


        /// <summary>
        /// verify GridViewCell's control type and its patterns.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private static bool VerifyGridViewCell(AutomationElement item, int row)
        {
            int column = 0;
            for (AutomationElement child = TreeWalker.RawViewWalker.GetFirstChild(item);
                child != null; child = TreeWalker.RawViewWalker.GetNextSibling(child))
            {
                if (!CheckGridItemPattern(child, column, row))
                    return false;
                if (!CheckTableItemPattern(child, column, row))
                    return false;

                ++column;
                if (CheckClassName(child, "ContentPresenter"))
                {
                    if (!VerifyControlType(child, ControlType.Custom))
                        return false;
                }
                else
                {
                    if (!VerifyControlType(child, ControlType.Text))
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Check GridItemPattern
        /// </summary>
        /// <param name="item"></param>
        /// <param name="column"></param>
        /// <param name="row"></param>
        /// <returns></returns>
        private static bool CheckGridItemPattern(AutomationElement item, int column, int row)
        {
            GridItemPattern pattern = (GridItemPattern)item.GetCurrentPattern(GridItemPattern.Pattern);

            UIVerifyLogger.LogComment(string.Format("Grid.Column: Exp: {0} Act: {1}", column, pattern.Current.Column));
            if (pattern.Current.Column != column)
            {
                return false;
            }
            UIVerifyLogger.LogComment(string.Format("Grid.ColumnSpan: Exp: {0} Act: {1}", 1, pattern.Current.ColumnSpan));
            if (pattern.Current.ColumnSpan != 1)
            {
                return false;
            }
            UIVerifyLogger.LogComment(string.Format("Grid.Row: Exp: {0} Act: {1}", row, pattern.Current.Row));
            if (pattern.Current.Row != row)
            {
                return false;
            }
            UIVerifyLogger.LogComment(string.Format("Grid.RowSpan: Exp: {0} Act: {1}", 1, pattern.Current.RowSpan));
            if (pattern.Current.RowSpan != 1)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Check TableItemPattern
        /// </summary>
        /// <param name="item"></param>
        /// <param name="column"></param>
        /// <param name="row"></param>
        /// <returns></returns>
        private static bool CheckTableItemPattern(AutomationElement item, int column, int row)
        {
            TableItemPattern pattern = (TableItemPattern)item.GetCurrentPattern(TableItemPattern.Pattern);
            UIVerifyLogger.LogComment(string.Format("Table.Column: Exp: {0} Act: {1}", column, pattern.Current.Column));
            if (pattern.Current.Column != column)
            {
                return false;
            }
            UIVerifyLogger.LogComment(string.Format("Table.ColumnSpan: Exp: {0} Act: {1}", 1, pattern.Current.ColumnSpan));
            if (pattern.Current.ColumnSpan != 1)
            {
                return false;
            }
            UIVerifyLogger.LogComment(string.Format("Table.Row: Exp: {0} Act: {1}", row, pattern.Current.Row));
            if (pattern.Current.Row != row)
            {
                return false;
            }
            UIVerifyLogger.LogComment(string.Format("Table.RowSpan: Exp: {0} Act: {1}", 1, pattern.Current.RowSpan));
            if (pattern.Current.RowSpan != 1)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// check that an automation element's ClassName is a specific name
        /// </summary>
        /// <param name="element">automation element</param>
        /// <param name="expectedClassName">expected class name</param>
        /// <returns></returns>        
        private static bool CheckClassName(AutomationElement element, string expectedClassName)
        {
            string actualClassName = (string)element.GetCurrentPropertyValue(AutomationElement.ClassNameProperty);
            return actualClassName == expectedClassName;
        }

        /// <summary>
        /// verify an automation element's ControlType is a specific value
        /// </summary>
        /// <param name="element">automation element</param>
        /// <param name="expected">expected control type</param>
        /// <returns></returns>
        private static bool VerifyControlType(AutomationElement element, ControlType expected)
        {
            ControlType controlType = (ControlType)element.GetCurrentPropertyValue(AutomationElement.ControlTypeProperty);
            UIVerifyLogger.LogComment("ControlType: Exp.[" + expected.LocalizedControlType + "] Act.[" + controlType.LocalizedControlType + "]");
            return expected == controlType;
        }
        /// <summary>
        /// check if the element is a WPF datagrid
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        private static bool CheckWPFDataGridElement(AutomationElement element)
        {
            if (string.Equals("WPF", (string)element.GetCurrentPropertyValue(AutomationElement.FrameworkIdProperty), StringComparison.InvariantCulture)
                && string.Equals("DataGrid", (string)element.GetCurrentPropertyValue(AutomationElement.ClassNameProperty), StringComparison.InvariantCulture))
            {
                return true;
            }
            return false;
        }

        private static AutomationElement GetTheNextSibling(AutomationElement ae)
        {
            if (ae == null)
            {
                Logger.LogComment("The automationelement cannot be null!");
                return null;
            }
            return TreeWalker.ControlViewWalker.GetNextSibling(ae);
        }
        private static AutomationElement FindFirstChildByClassName(AutomationElement ae, string className)
        {
            if ((ae == null) || (string.IsNullOrEmpty(className)))
            {
                Logger.LogComment("The parameter is invalid!");
                return null;
            }
            return ae.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.ClassNameProperty, className));
        }
        private static AutomationElement GetTheLastChild(AutomationElement ae)
        {
            if (ae == null)
            {
                Logger.LogComment("The element cannot be null!");
                return null;
            }
            return TreeWalker.ControlViewWalker.GetLastChild(ae);
        }
        /// <summary>
        /// Checks whether ItemContainer pattern is supported
        /// </summary>
        /// <param name="element">automation element we are dealing with</param>
        /// <returns>true if pattern is supported and false if not </returns>
        private static bool CheckItemContainerPattternSupported(AutomationElement element)
        {
            try
            {
                element.GetCurrentPattern(ItemContainerPattern.Pattern);
            }
            catch
            {
                //pattern in not supported.  If this is Win7 or new UIACore is installed we need to fail the test.  Otherwise pass and quit.
                UIVerifyLogger.LogComment("ItemContainerPattern is not supported.  This indicates that either");
                UIVerifyLogger.LogComment("new v7 + UIAutomationCore.dll is not installed or there is a bug in which case the test will fail");

                return false;
            }
            return true;
        }
        /// <summary>
        /// Detects if the version of UIAutomationCore on the system is older than v7
        /// </summary>
        /// <returns></returns>
        private static bool CheckUiaCoreOld()
        {
            //Here we only check the system32 folder even on 64 bit OSs since our assumtion is that
            //if new UIA Core is installed it will be at leastin system32 folder.
            String file = Environment.ExpandEnvironmentVariables("%WINDIR%\\System32\\UIAutomationCore.dll");
            FileVersionInfo fileVersion = FileVersionInfo.GetVersionInfo(file);
            Logger.LogComment("The version of UIAutomationCore.dll is " + fileVersion.FileMajorPart);

            return (fileVersion.FileMajorPart < 7);
        }
        
        /// <summary>
        /// verifies conditions and devirtualizes AE
        /// </summary>
        /// <param name="targetElement">automation element we are dealing with</param>
        /// <returns>true if all conditions pass and no exceptions hit. otherwise fail</returns>
        private static bool DeVirtualizeItem(AutomationElement targetElement)
        {
            if (targetElement == null)
            {
                UIVerifyLogger.LogComment("Failed to find requested AutomationElement");
                return false;
            }

            //Can't currently virtualize TabItems so we'll skip here
            if ((string)targetElement.GetCurrentPropertyValue(AutomationElement.ClassNameProperty) == "TabItem")
            {
                return true;
            }

            VirtualizedItemPattern virtualizedItemPattern;
            try
            {
                virtualizedItemPattern = targetElement.GetCurrentPattern(VirtualizedItemPattern.Pattern) as VirtualizedItemPattern;
                virtualizedItemPattern.Realize();
            }
            catch
            {
                UIVerifyLogger.LogComment("VirtualizedItemPattern failed");
                return false;
            }
            
            //Need to sleep here to make sure the item is realized before any other action happens
            Thread.Sleep(2000);
            return true;
        }

        /// <summary>
        /// verifies conditions and devirtualizes AE
        /// this function does not enforce AE to be Virtualized before calling Realize()
        /// </summary>
        /// <param name="targetElement">AutomationElement we are dealing with</param>
        /// <returns>true if all conditions pass and no exceptions hit. otherwise fail</returns>
        private static bool DeVirtualizeItemAndVerify(AutomationElement targetElement)
        {
            if (targetElement == null)
            {
                UIVerifyLogger.LogComment("Failed to find requested AutomationElement");
                return false;
            }
            //Can't currently virtualize TabItems so we'll skip here
            if ((string)targetElement.GetCurrentPropertyValue(AutomationElement.ClassNameProperty) == "TabItem")
            {
                return true;
            }

            VirtualizedItemPattern virtualizedItemPattern;

            try
            {
                //Check if BoundingRectangle property is available.  It will tell us if the item we found is virtualized or not
                object boundingRectangle = targetElement.GetCurrentPropertyValue(AutomationElement.BoundingRectangleProperty);
                UIVerifyLogger.LogComment("If bounding rectangle is available then the item we are testing");
                UIVerifyLogger.LogComment("is not virtualized and the test will fail");
                return false;
            }
            catch
            {
                UIVerifyLogger.LogComment("Can't get the Bounding Rectangle therefore the item is virtualized");
            }
            
            try
            {
                //Second check makes sure that VirtualizedItmePattern is available.  It is another indication that the item is virtualized
                virtualizedItemPattern = targetElement.GetCurrentPattern(VirtualizedItemPattern.Pattern) as VirtualizedItemPattern;
                UIVerifyLogger.LogComment("No exception is thrown, VirtualizedItemPatter is available, " + targetElement.Current.Name + " is virtualized");
            }
            catch (Exception e)
            {
                UIVerifyLogger.LogComment(e);
                UIVerifyLogger.LogComment("Can't get VirtualizedItemPatter on " + targetElement.Current.Name + ". This item is most likely not virtualized");
                UIVerifyLogger.LogComment("Virtualized item is expected so test will fail");
                return false;
            }

            if (!DeVirtualizeItem(targetElement))
            {
                UIVerifyLogger.LogComment("Failed to Realize AutomationElement" + targetElement);
                return false;
            }
            try
            {
                object boundingRectangle = targetElement.GetCurrentPropertyValue(AutomationElement.BoundingRectangleProperty);
                UIVerifyLogger.LogComment("If bounding rectangle is available then the item is not virtualized");
            }
            catch
            {
                UIVerifyLogger.LogComment("Can't get the Bounding Rectangle therefore the item is still virtualized");
                return false;
            }
            
            try
            {
                //Second check makes sure that VirtualizedItmePattern is not available.  It is another indication that the item is de-virtualized
                virtualizedItemPattern = targetElement.GetCurrentPattern(VirtualizedItemPattern.Pattern) as VirtualizedItemPattern;
                UIVerifyLogger.LogComment("No exception is thrown, VirtualizedItemPatter is available, " + targetElement.Current.Name + " is virtualized");
                UIVerifyLogger.LogComment("Virtualized item is not expected so test will fail");
                return false;
            }
            catch (Exception e)
            {
                UIVerifyLogger.LogComment(e);
                UIVerifyLogger.LogComment("Can't get VirtualizedItemPatter on " + targetElement.Current.Name + ". This indicates that the item is not virtualized");
            }
            return true;
        }
        #endregion
    }
}
