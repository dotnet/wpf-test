using System;
using System.Windows.Controls;
using System.Windows.Threading;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// <description>
    /// ListBoxRegressionTest59
    /// </description>
    /// </summary>
    [Test(1, "ListBox", "ListBoxRegressionTest59", Versions = "4.0+,4.0Client+")]
    public class ListBoxRegressionTest59 : XamlTest
    {
        #region Private Members

        private ListBox listbox;

        #endregion

        #region Public Members

        public ListBoxRegressionTest59()
            : base(@"ListBoxRegressionTest59.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(Repro);
        }

        public TestResult Setup()
        {
            Status("Setup");

            listbox = (ListBox)RootElement.FindName("listbox");

            // Add a long content item
            ListBoxItem longItem = new ListBoxItem();
            longItem.Content = "This is an item with a very long name that will cause a scrollbar to appear in the listbox";
            listbox.Items.Add(longItem);
            DispatcherOperations.WaitFor(DispatcherPriority.ApplicationIdle);

            LogComment("Setup was successful");

            return TestResult.Pass;
        }

        public TestResult CleanUp()
        {
            listbox = null;
            return TestResult.Pass;
        }

        /// <summary>
        /// Ensure all items have the same ActualWidth
        /// </summary>
        public TestResult Repro()
        {
            if (listbox.Items.Count == 0)
            {
                throw new TestValidationException("Fail: listbox has not item.");
            }

            ListBoxItem firstItem = listbox.Items[0] as ListBoxItem;
            double expectedItemWidth = firstItem.ActualWidth;

            foreach (ListBoxItem item in listbox.Items)
            {
                if (item.ActualWidth != expectedItemWidth)
                {
                    throw new Exception(String.Format("Fail: item[{0}] ActualWidth:{1} != firstItemWidth:{2}",
                        listbox.Items.IndexOf(item), item.ActualWidth, expectedItemWidth));
                }
            }

            return TestResult.Pass;
        }

        #endregion
    }
}
