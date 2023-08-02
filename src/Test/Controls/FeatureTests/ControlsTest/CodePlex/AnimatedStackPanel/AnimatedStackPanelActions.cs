using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;
using System.Xml;

using WpfControlToolkit;

using Microsoft.Test;
using Microsoft.Test.Input;
using Microsoft.Test.Logging;
using Microsoft.Test.Threading;
using Avalon.Test.ComponentModel.Utilities;


namespace Avalon.Test.ComponentModel.Actions
{
    public static class AnimatedStackPanelActions
    {
        #region Private Data
        private const string                        NEW_ITEM    = "NewItem";
        #endregion


        #region Public and Protected Members
        /******************************************************************************
        * Function:          ListBoxAddOrRemoveItem
        ******************************************************************************/
        /// <summary>
        /// Testing the AnimatedStackPanel control.  Use event handlers to insert or remove items,
        /// which will then automatically invoke the custom Animation.  Verification consists of
        /// simply checking that the add or remove took place correctly.
        /// </summary>
        /// <param name="targetElement">The ListBox in Markup to be tested</param>
        /// <param name="addOrRemoveAction">The action to be taken: Add or Remove</param>
        /// <param name="startingIndex">The start position of the Add or Remove action</param>
        /// <param name="addOrRemoveCount">The number of items to be added or removed from the list</param>
        /// <returns>A passing or failing result</returns>
        public static bool ListBoxAddOrRemoveItem(ListBox targetElement, string addOrRemoveAction, int startingIndex, int addOrRemoveCount)
        {
            TestLog.Current.LogStatus("*************TEST PARAMETERS*************");
            TestLog.Current.LogStatus("*  AddOrRemoveAction: " + addOrRemoveAction);
            TestLog.Current.LogStatus("*  StartingIndex:     " + startingIndex);
            TestLog.Current.LogStatus("*  AddOrRemoveCount:  " + addOrRemoveCount);
            TestLog.Current.LogStatus("******************************************");

            ArrayList expectedList = CreateExpectedList((ItemsControl)targetElement, addOrRemoveAction, startingIndex, addOrRemoveCount);

            InvokeAnimation((ListBox)targetElement, addOrRemoveAction, startingIndex, addOrRemoveCount);

            QueueHelper.WaitTillQueueItemsProcessed();

            return VerifyAnimatedStackPanel((ItemsControl)targetElement, expectedList);
        }

        /******************************************************************************
        * Function:          ComboBoxAddOrRemoveItem
        ******************************************************************************/
        /// <summary>
        /// Testing the AnimatedStackPanel control.  Use event handlers to insert or remove items,
        /// which will then automatically invoke the custom Animation.  Verification consists of
        /// simply checking that the add or remove took place correctly.
        /// </summary>
        /// <param name="targetElement">The ComboBox in Markup to be tested</param>
        /// <param name="action">The action to be taken: Add or Remove</param>
        /// <param name="startingIndex">The start position of the Add or Remove action</param>
        /// <param name="addOrRemoveCount">The number of items to be added or removed from the list</param>
        /// <returns>A passing or failing result</returns>
        public static bool ComboBoxAddOrRemoveItem(ComboBox targetElement, string addOrRemoveAction, int startingIndex, int addOrRemoveCount)
        {
            TestLog.Current.LogStatus("*************TEST PARAMETERS*************");
            TestLog.Current.LogStatus("*  AddOrRemoveAction: " + addOrRemoveAction);
            TestLog.Current.LogStatus("*  StartingIndex:     " + startingIndex);
            TestLog.Current.LogStatus("*  AddOrRemoveCount:  " + addOrRemoveCount);
            TestLog.Current.LogStatus("******************************************");

            ArrayList expectedList = CreateExpectedList((ItemsControl)targetElement, addOrRemoveAction, startingIndex, addOrRemoveCount);

            InvokeAnimation((ComboBox)targetElement, addOrRemoveAction, startingIndex, addOrRemoveCount);

            QueueHelper.WaitTillQueueItemsProcessed();

            return VerifyAnimatedStackPanel((ItemsControl)targetElement, expectedList);
        }
        #endregion


        #region Private Members
        /******************************************************************************
        * Function:          InvokeAnimation (ListBox)
        ******************************************************************************/
        /// <summary>
        /// Adds or removes an item from the ListBox, which will invoke the AnimatedStackPanel animation.
        /// Also, determines the expected values for the contents of the ListBox.
        /// </summary>
        /// <param name="listBox">The ListBox beging tested</param>
        /// <returns></returns>
        private static void InvokeAnimation(ListBox listBox, string addOrRemoveAction, int startingIndex, int addOrRemoveCount)          
        {
            if (addOrRemoveAction.ToLowerInvariant() == "add")
            {
                for (int i=0; i<addOrRemoveCount; i++)
                {
                    TestLog.Current.LogStatus("ACT: Adding at: " + startingIndex.ToString());
                    listBox.Items.Insert(startingIndex, NEW_ITEM);
                }
            }
            else if (addOrRemoveAction.ToLowerInvariant() == "remove")
            {
                for (int j=0; j<addOrRemoveCount; j++)
                {
                    TestLog.Current.LogStatus("ACT: Removing at: " + startingIndex.ToString());
                    listBox.Items.RemoveAt(startingIndex);
                    QueueHelper.WaitTillTimeout(TimeSpan.FromMilliseconds(250));
                }
            }
        }

        /******************************************************************************
        * Function:          InvokeAnimation (ComboBox)
        ******************************************************************************/
        /// <summary>
        /// Adds or removes an item from the ComboBox, which will invoke the AnimatedStackPanel animation.
        /// Also, determines the expected values for the contents of the ComboBox.
        /// </summary>
        /// <param name="comboBox">The ComboBox beging tested</param>
        /// <returns></returns>
        private static void InvokeAnimation(ComboBox comboBox, string addOrRemoveAction, int startingIndex, int addOrRemoveCount)          
        {
            if (addOrRemoveAction.ToLowerInvariant() == "add")
            {
                for (int i=0; i<addOrRemoveCount; i++)
                {
                    comboBox.Items.Insert(startingIndex, NEW_ITEM);
                }
            }
            else if (addOrRemoveAction.ToLowerInvariant() == "remove")
            {
                for (int j=0; j<addOrRemoveCount; j++)
                {
                    comboBox.Items.RemoveAt(startingIndex);
                    QueueHelper.WaitTillTimeout(TimeSpan.FromMilliseconds(250));
                }
            }
        }

        /******************************************************************************
        * Function:          CreateExpectedList
        ******************************************************************************/
        /// <summary>
        /// Determines the expected values for the contents of the ItemsControl by matching the
        /// requestion actions (Add or Remove) on a list of Expected items, which matches the actual
        /// list specied in Markup in the .xtc file.
        /// </summary>
        /// <returns></returns>
        private static ArrayList CreateExpectedList(ItemsControl control, string addOrRemoveAction, int startingIndex, int addOrRemoveCount)          
        {
            ArrayList expItems = new ArrayList();

            for (int i=0; i<control.Items.Count; i++)
            {
                expItems.Add("Item " + i.ToString());  //Matches content in Markup.
            }

            if (addOrRemoveAction.ToLowerInvariant() == "add")
            {
                for (int i=0; i<addOrRemoveCount; i++)
                {
                    expItems.Insert(startingIndex, NEW_ITEM);
                }
            }
            else if (addOrRemoveAction.ToLowerInvariant() == "remove")
            {
                for (int j=0; j<addOrRemoveCount; j++)
                {
                    expItems.RemoveAt(startingIndex);
                }
            }
            else
            {
                throw new Exception(" ERROR!!! Incorrect Action specified in the .xtc file.");
            }
            
            return expItems;
        }


        /******************************************************************************
        * Function:          VerifyAnimatedStackPanel
        ******************************************************************************/
        /// <summary>
        /// Verifies the result of the test case, and returns a Pass/Fail result.
        /// </summary>
        /// <returns>A boolean, indicating whether or not the test case passed</returns>
        private static bool VerifyAnimatedStackPanel(ItemsControl itemsControl, ArrayList expItems)
        {
            ArrayList actItems = new ArrayList();;

            for (int i=0; i<itemsControl.Items.Count; i++)
            {
                actItems.Add(itemsControl.Items[i]);
            }

            TestLog.Current.LogStatus("*************************************************");
            bool listCorrect = CompareLists(actItems, expItems);
            TestLog.Current.LogStatus("*************************************************");

            return listCorrect;
        }

        /******************************************************************************
        * Function:          CompareLists
        ******************************************************************************/
        /// <summary>
        /// Compares the contents of two ArrayLists of strings.
        /// </summary>
        /// <param name="actualList">A list to be compared</param>
        /// <param name="expectedList">A list to be compared</param>
        /// <returns>A boolean, indicating whether or not the two lists match</returns>
        private static bool CompareLists(ArrayList actualList, ArrayList expectedList)
        {
            bool comparisonResult = true;

            if (actualList.Count != expectedList.Count)
            {
                TestLog.Current.LogEvidence("FAIL: the length of the lists do not match.");
                TestLog.Current.LogEvidence("Actual Count:   " + actualList.Count);
                TestLog.Current.LogEvidence("Expected Count: " + expectedList.Count);
                comparisonResult = false;
            }
            else
            {
                for (int j=0; j<actualList.Count; j++)
                {
                    TestLog.Current.LogEvidence("Item #" + j.ToString() + "-- Actual List: " + actualList[j] + " / Expected List: " + expectedList[j]);
                    if ((string)actualList[j] != (string)expectedList[j])
                    {
                        comparisonResult = false;
                        break;
                    }
                }
            }
            return comparisonResult;
        }
        #endregion
    }
}


