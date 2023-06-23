using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Threading;

using Microsoft.Test;
using Microsoft.Test.Input;
using Microsoft.Test.Logging;
using Microsoft.Test.Threading;

using WpfControlToolkit;

namespace Avalon.Test.ComponentModel.Actions
{
    using PropertyName = ListPagerPropertyName;

    /// <summary>
    /// Enum values corresponding to ListPager property names
    /// </summary>
    internal enum ListPagerPropertyName
    {
        currentPageCount,
        currentPageIndex,
        maxPageIndex,
        pageCount,
        pageSize
    }

    internal static class ListPagerTestValidationCommon
    {
        // The ListPager has 4 built-in commands {first, last, next, prev}
        internal const int builtInCmdCount = 4;

        /// <summary>
        /// Method to compare the "CanExecute()" state of all ListPager's exposed UIListPagerCommand objects.
        /// The number of exposed commands == (builtInCmdCount + (number of pages))
        /// </summary>
        /// <param name="thePager">The ListPager instance</param>
        /// <param name="expectedExecuteValues">expected values of "CanExecute()" for ListPager's exposed commands</param>
        /// <returns></returns>
        internal static bool InternalValidateCommandsState(UIListPager thePager, bool[] expectedExecuteValues)
        {
            bool retval = true;

            int expectedCommandCount = expectedExecuteValues.Length;
            bool[] actualExecute = new bool[expectedCommandCount];
            IList<UIListPagerCommand> pageCommands = thePager.PageCommands;

            if (pageCommands.Count + builtInCmdCount != expectedCommandCount)
            {
                TestLog.Current.LogEvidence("!!!ERROR: Count of pageCommands is unexpected value.");
                TestLog.Current.LogEvidence("..Detail: Actual count= "
                    + pageCommands.Count.ToString() + "; Expected count = " + (expectedCommandCount - builtInCmdCount).ToString());
                retval = false;
            }
            else
            {

                if (thePager.FirstCommand.CanExecute(null) != expectedExecuteValues[0])
                {
                    TestLog.Current.LogEvidence("!!!ERROR: FirstCommand.CanExecute() is unexpected value.");
                    TestLog.Current.LogEvidence("..Detail: Actual value == "
                        + thePager.FirstCommand.CanExecute(null).ToString() + "; Expected value == " + expectedExecuteValues[0].ToString());
                    retval = false;
                }
                else if (thePager.LastCommand.CanExecute(null) != expectedExecuteValues[1])
                {
                    TestLog.Current.LogEvidence("!!!ERROR: LastCommand.CanExecute() is unexpected value.");
                    TestLog.Current.LogEvidence("..Detail: Actual value == "
                        + thePager.FirstCommand.CanExecute(null).ToString() + "; Expected value == " + expectedExecuteValues[0].ToString());
                    retval = false;
                }
                else if (thePager.NextCommand.CanExecute(null) != expectedExecuteValues[2])
                {
                    TestLog.Current.LogEvidence("!!!ERROR: NextCommand.CanExecute() is unexpected value.");
                    TestLog.Current.LogEvidence("..Detail: Actual value == "
                        + thePager.FirstCommand.CanExecute(null).ToString() + "; Expected value == " + expectedExecuteValues[0].ToString());
                    retval = false;
                }
                else if (thePager.PreviousCommand.CanExecute(null) != expectedExecuteValues[3])
                {
                    TestLog.Current.LogEvidence("!!!ERROR: PreviousCommand.CanExecute() is unexpected value.");
                    TestLog.Current.LogEvidence("..Detail: Actual value == "
                        + thePager.FirstCommand.CanExecute(null).ToString() + "; Expected value == " + expectedExecuteValues[0].ToString());
                    retval = false;
                }
                else
                {
                    int i = builtInCmdCount;
                    foreach (UIListPagerCommand cmd in pageCommands)
                    {
                        if (cmd.CanExecute(null) != expectedExecuteValues[i++])
                        {
                            TestLog.Current.LogEvidence("!!!ERROR: Page #" + (i - builtInCmdCount).ToString() + " command.CanExecute() is unexpected value.");
                            TestLog.Current.LogEvidence("..Detail: Actual value == "
                                + cmd.CanExecute(null).ToString() + "; Expected value == " + expectedExecuteValues[i - 1].ToString());
                            retval = false;
                            break;
                        }
                    }
                }
            }
            return retval;
        }

        /// <summary>
        /// Method to compare ListPager's properties with expected value for those properties, not including UIListPagerCommand values.
        /// </summary>
        /// <param name="thePager">The ListPager instance</param>
        /// <param name="expectedItemsSource">The IList instance previously used to initialize this ListPager instance</param>
        /// <param name="expected">Dictionary of expected [property-name key, expected-value] pairs</param>
        /// <returns></returns>
        internal static bool InternalValidateBasicState(UIListPager thePager, IList expectedItemsSource, Dictionary<PropertyName, int> expected)
        {
            bool retval = true;

            int expectedCurrentPageCount = (int)expected[PropertyName.currentPageCount];
            int expectedCurrentPageIndex = (int)expected[PropertyName.currentPageIndex];
            int expectedMaxPageIndex = (int)expected[PropertyName.maxPageIndex];
            int expectedPageCount = (int)expected[PropertyName.pageCount];
            int expectedPageSize = (int)expected[PropertyName.pageSize];

            if (!object.ReferenceEquals(expectedItemsSource, thePager.ItemsSource))
            {
                TestLog.Current.LogEvidence("!!!ERROR: ItemsSource property not referenceEqual to ItemsSource argument.");
                retval = false;
            }
            else if (null == (thePager.CurrentPage as IList))
            {
                TestLog.Current.LogEvidence("!!!ERROR: CurrentPage property is null.");
                retval = false;
            }
            else if (expectedCurrentPageCount != thePager.CurrentPage.Count)
            {
                TestLog.Current.LogEvidence("!!!ERROR: Count of elements on CurrentPage is not the expected value.");
                TestLog.Current.LogEvidence("..Detail: Actual value = "
                    + thePager.CurrentPage.Count.ToString() + "; Expected value= " + expectedCurrentPageCount.ToString());
                retval = false;
            }
            else if (thePager.CurrentPageIndex != expectedCurrentPageIndex)
            {
                TestLog.Current.LogEvidence("!!!ERROR: CurrentPageIndex is not the expected value.");
                TestLog.Current.LogEvidence("..Detail: Actual value = "
                    + thePager.CurrentPageIndex.ToString() + "; Expected value= " + expectedCurrentPageIndex.ToString());
                retval = false;
            }
            else if (thePager.MaxPageIndex != expectedMaxPageIndex)
            {
                TestLog.Current.LogEvidence("!!!ERROR: MaxPageIndex is not the expected value.");
                TestLog.Current.LogEvidence("..Detail: Actual value = "
                    + thePager.MaxPageIndex.ToString() + "; Expected value= " + expectedMaxPageIndex.ToString());
                retval = false;
            }
            else if (thePager.PageCount != expectedPageCount)
            {
                TestLog.Current.LogEvidence("!!!ERROR: PageCount is not the expected value.");
                TestLog.Current.LogEvidence("..Detail: Actual value = "
                    + thePager.PageCount.ToString() + "; Expected value= " + expectedPageCount.ToString());
                retval = false;
            }
            else if (thePager.PageSize != expectedPageSize)
            {
                TestLog.Current.LogEvidence("!!!ERROR: PageSize is not the expected value.");
                TestLog.Current.LogEvidence("..Detail: Actual value = "
                    + thePager.PageSize.ToString() + "; Expected value= " + expectedPageSize.ToString());
                retval = false;
            }
            return retval;
        }
    }
}
