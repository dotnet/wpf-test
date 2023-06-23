using Avalon.Test.ComponentModel.Utilities;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// <description>
    /// Regression Test :Use ListView with custom item height and setting virtualizationmode to recycling causing UI freeze when scrolling
    /// </description>
    /// </summary>
    [Test(0, "ScrollViewer", "ListViewScrollingRegressionTest68", SecurityLevel = TestCaseSecurityLevel.FullTrust, Timeout = 30, Versions = "4.5.1+")]
    public class ListViewScrollingRegressionTest68 : XamlTest
    {
        #region Private Date

        private ListView myListView;
        private ScrollViewer myScrollViewer;

        #endregion

        #region Constructor

        public ListViewScrollingRegressionTest68()
            : base(@"ListViewScrollingRegressionTest68.xaml")
        {
            InitializeSteps += new TestStep(SetUp);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(ListViewScrollingTest);
        }

        #endregion

        #region Test Steps

        public TestResult SetUp()
        {
            Status("SetUp specific for ListViewScrollingRegressionTest68");

            myListView = RootElement.FindName("ListView") as ListView;
            Assert.AssertTrue("Failed to find myListView from xaml", myListView != null);

            myScrollViewer = VisualTreeHelper.GetVisualChild<ScrollViewer, ListView>(myListView);
            Assert.AssertTrue("Failed to find myScrollViewer from xaml", myScrollViewer != null);

            List<ShortLongData> dataList = Enumerable.Range(0, 20).Select(i => new ShortLongData(i)).ToList();
            Assert.AssertTrue("Failed to setup datalist detials source", dataList != null);

            myListView.ItemsSource = dataList;
            this.Window.WindowState = WindowState.Maximized;
            DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);

            LogComment("SetUp specific for ListViewScrollingRegressionTest68 was successful");
            return TestResult.Pass;
        }

        public TestResult CleanUp()
        {
            myListView = null;
            myScrollViewer = null;

            return TestResult.Pass;
        }

        public TestResult ListViewScrollingTest()
        {
            Status("ListViewScrollingTest");

            this.Window.Focus();

            LogComment("Scrolling myScrollViewer five times");
            //Without bug fix,UI will freeze after two times scrolling
            for (int i = 0; i < 5; i++)
            {
                myScrollViewer.ScrollToEnd();
                DispatcherHelper.DoEvents(500);
                myScrollViewer.ScrollToHome();
                DispatcherHelper.DoEvents(500);
            }

            LogComment("ListViewScrollingTest was sucessful");
            return TestResult.Pass;
        }

        #endregion

        #region Helper Classes

        public class ShortLongData
        {
            public string CategoryName { get; set; }
            public string ShortDescription { get; set; }

            public ShortLongData(int i)
            {
                CategoryName = "Category Name";
                ShortDescription = "Short Description";
                for (int j = 0; j < i * 17 % 50; j++)
                {
                    ShortDescription += "\nHello New Line";
                }
            }
        }

        #endregion
    }
}
