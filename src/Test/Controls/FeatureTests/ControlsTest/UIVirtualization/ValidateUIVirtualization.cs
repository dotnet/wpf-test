using System;
using System.Collections;
using System.Diagnostics;
using System.Windows;
using System.Reflection;
using System.Windows.Controls;
using Avalon.Test.ComponentModel.Utilities;

using Microsoft.Test.Logging;

namespace Avalon.Test.ComponentModel.Validations
{

    /// <summary>
    /// This validation verifies that ListBox contains Virtualized and DeVirtualizedItems
    /// </summary>
    public class ValidateUIVirtualization : IValidation
    {

        private bool result = true;
        private ListBox listBox = null;
        private int numberOfVirtualizedItems = 0;
        private int numberOfDeVirtualizedItems = 0;

        public bool Validate(params object[] validationParams)
        {
            listBox = validationParams[0] as ListBox;

            for (int i = 0; i < listBox.Items.Count; i++)
            {
                if ((listBox.ItemContainerGenerator.ContainerFromItem(listBox.Items[i]) as ListBoxItem) != null)
                {
                    numberOfDeVirtualizedItems++;
                }
                else
                {
                    numberOfVirtualizedItems++;
                }
            }

            TestLog.Current.LogStatus("No of DeVirtualized items " + numberOfDeVirtualizedItems);
            TestLog.Current.LogStatus("No of Virtualized items " + numberOfVirtualizedItems);

            if (numberOfDeVirtualizedItems < 1)
            {
                TestLog.Current.LogEvidence("FAIL: expected at least one DeVirtualized item");
                result = false;
            }

            if (numberOfVirtualizedItems < 1)
            {
                TestLog.Current.LogEvidence("FAIL: expected at least one Virtualized item");
                result = false;
            }

            return result;
        }
    }

    /// <summary>
    /// This validation verifies that at number of generated listbox items ( devirtualized items) are less than 
    /// certain number
    /// </summary>
    public class ValidateMaxDeVirtualizedItems : IValidation
    {
        private bool result = true;
        private ListBox listBox = null;
        private int maxDeVirtualizedItems = 0;
        private int numberOfDeVirtualizedItems = 0;

        public bool Validate(params object[] validationParams)
        {
            listBox = validationParams[0] as ListBox;
            object[] param = validationParams[1] as object[];
            maxDeVirtualizedItems = Convert.ToInt32(param[0], System.Globalization.CultureInfo.InvariantCulture);

            for (int i = 0; i < listBox.Items.Count; i++)
            {
                if ((listBox.ItemContainerGenerator.ContainerFromItem(listBox.Items[i]) as ListBoxItem) != null)
                {
                    numberOfDeVirtualizedItems++;
                }
            }

            if (numberOfDeVirtualizedItems > maxDeVirtualizedItems)
            {
                TestLog.Current.LogEvidence("FAIL: expected number of devirtualized items less than " + maxDeVirtualizedItems);
                TestLog.Current.LogEvidence("FAIL: actual number of devirtualized items  " + numberOfDeVirtualizedItems);
                return false;
            }

            return result;
        }
    }

    /// <summary>
    /// Verify last item devirtualized
    /// </summary>
    public class ValidateLastItemDeVirtualized : IValidation
    {
        private ListBox listBox = null;

        public bool Validate(params object[] validationParams)
        {
            listBox = validationParams[0] as ListBox;

            if ((listBox.ItemContainerGenerator.ContainerFromItem(listBox.Items[listBox.Items.Count - 1]) as ListBoxItem) != null)
            {
                TestLog.Current.LogStatus("ListBox last item " + listBox.Items[listBox.Items.Count - 1].ToString() + " is DeVirtualized");
                return true;
            }
            else
            {
                TestLog.Current.LogEvidence("FAIL:  ListBox last item " + listBox.Items[listBox.Items.Count - 1].ToString() + " is not DeVirtualized");
                return false;
            }
        }
    }

    /// <summary>
    /// Validate cancelling CleanUp keep the generated container for items in ListBox
    /// </summary>
    public class ValidateCleanupEventCancel : IValidation
    {
        public bool Validate(params object[] validationParams)
        {

            ListBox listBox = validationParams[0] as ListBox;
            TestLog.Current.LogStatus(listBox.ToString());
            object[] param = validationParams[1] as object[];

            string argsKey = param[0] as string;

            ArrayList args = StateTable.Get(argsKey) as ArrayList;

            if (args == null)
            {
                TestLog.Current.LogEvidence("Value returned for Key " + argsKey + " is null");
                return false;
            }
            
            int numberOfDeVirtualizedItems = 0;

            for (int i = 0; i < listBox.Items.Count; i++)
            {
                if ((listBox.ItemContainerGenerator.ContainerFromItem(listBox.Items[i]) as ListBoxItem) != null)
                {
                    numberOfDeVirtualizedItems++;
                }
            }

            TestLog.Current.LogEvidence("Expected at least : " + + args.Count + " : number of DeVirtualizedItems");
            TestLog.Current.LogEvidence("Actual number of DeVirtualizedItems: " + numberOfDeVirtualizedItems);

            if (args.Count > numberOfDeVirtualizedItems)
            {
                TestLog.Current.LogEvidence("FAIL: Actual number of DeVirtualizedItems is less than to expected number of DeVirtualizedItems");
                return false;
            }

            return true;
        }
    }


    /// <summary>
    /// Verify last item devirtualized
    /// </summary>
    public class ValidateLastItemSelected : IValidation
    {
        private ListBox listBox = null;

        public bool Validate(params object[] validationParams)
        {
            listBox = validationParams[0] as ListBox;

            if ( listBox.Items[listBox.Items.Count - 1] == listBox.SelectedItem )
            {
                TestLog.Current.LogStatus("PASS: ListBox last item is the SelectedItem");
                return true;
            }
            else
            {
                TestLog.Current.LogEvidence("FAIL: ListBox last item is not Selected");
                return false;
            }
        }
    }

    /// <summary>
    /// Verify Selected Index increased/decreased as expected
    /// </summary>
    public class ValidateSelectedIndexChange : IValidation
    {
        private ListBox listBox = null;

        public bool Validate(params object[] validationParams)
        {
            if (validationParams == null)
            {
                TestLog.Current.LogEvidence("FAIL - validationParams == null");
                return false;
            }

            if (validationParams.Length < 2)
            {
                TestLog.Current.LogEvidence("FAIL - Expected validationParams.Length <= 2, Actual: " + validationParams.Length);
                return false;
            }

            listBox = validationParams[0] as ListBox;
            object[] param = validationParams[1] as object[];

            if (param.Length < 3)
            {
                TestLog.Current.LogEvidence("FAIL - Expected param.Length <= 3, Actual: " + param.Length);
                return false;
            }

            //Key to retrieve previous index from StateTable
            string argsKey = param[0].ToString();
            //SelectedIndex should have increased or decreased
            bool increasingDirection = bool.Parse(param[1].ToString());
            //What's the expected minimum delta between previous and current index
            int minimumDelta = Int32.Parse(param[2].ToString());
            
            int previousIndex = Int32.Parse(StateTable.Get(argsKey).ToString());

            TestLog.Current.LogStatus("Previous Index: " + previousIndex);
            TestLog.Current.LogStatus("Current Index: " + listBox.SelectedIndex);

            if (increasingDirection)
            {
                if (previousIndex >= listBox.SelectedIndex)
                {
                    TestLog.Current.LogEvidence("FAIL: ListBox SelectedIndex expected to increase");
                    return false;
                }
            }
            else
            {
                if (previousIndex <= listBox.SelectedIndex)
                {
                    TestLog.Current.LogEvidence("FAIL: ListBox SelectedIndex expected to decrease");
                    return false;
                }
            }

            TestLog.Current.LogStatus("Expected minimum difference : " + minimumDelta.ToString());
            TestLog.Current.LogStatus("Actual difference : " + Math.Abs(previousIndex - listBox.SelectedIndex).ToString());

            if (Math.Abs(previousIndex - listBox.SelectedIndex) < minimumDelta)
            {
                TestLog.Current.LogEvidence("FAIL: Difference between current SelectedIndex and previous Index is less than expected");
                return false;
            }

            TestLog.Current.LogEvidence("PASS:ValidateSelectedIndexChange");
            return true;
        }
    }


}


